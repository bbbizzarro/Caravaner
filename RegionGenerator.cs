using System;
using System.Collections;
using System.Collections.Generic;
using Godot;
using Caravaner;
using System.Linq;

public class RegionGenerator {
	RandomNumberGenerator rng = new RandomNumberGenerator();

	public int Width = 10;
	public int Height = 10;
	public float WorldScale = 64f;
	public GridMap gridMap;

	Vector2Int offset;
	int GridSize = 15;
	HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
	OpenSimplexNoise noise = new OpenSimplexNoise();
	public bool adjacenciesSet = false;
	PathFinder pathFinder;
	Dictionary<Region, RegRandData> randData = new Dictionary<Region, RegRandData>();
	bool Success = true;

	public RegionGenerator() {

	}

	public RegionGenerator(int regionGridWidth, int regionGridHeight,
						   int regionGridTileSize, float worldTileSize) {
		Width = regionGridHeight;
		Height = regionGridHeight;
		GridSize = regionGridTileSize;
		WorldScale = worldTileSize;
		rng.Randomize();
	}

	public RegionGenerator(int regionGridWidth, int regionGridHeight,
						   int regionGridTileSize, float worldTileSize, ulong seed) {
		Width = regionGridHeight;
		Height = regionGridHeight;
		GridSize = regionGridTileSize;
		WorldScale = worldTileSize;
		rng.Seed = seed;
	}

	private struct RegRandData {
		public Color color;
		public float growthSlow;
	}

	public IEnumerable<Region> GetRegions() {
		return gridMap.GetRegions();
	}

	public int NumbActiveRegions() {
		return gridMap.NumActiveRegions;
	}

	// Generation entry point
	private void CreateRegionCenters() {
		offset = new Vector2Int(Width / 2, Height / 2);
		gridMap = new GridMap(Width * GridSize, Height * GridSize, WorldScale);
		pathFinder = new PathFinder(100, gridMap);
		adjacenciesSet = false;
		Vector2Int cBuffer = new Vector2Int(1, -1);
		GD.Print(String.Format("Creating new map:"));
		GD.Print(String.Format("Grid Width: {0} Grid Height: {1}", 
			Width * GridSize, Height * GridSize));
		// Generate region center points within square grid.
		for (int i = 0; i < Width; ++i) { 
			for (int j = 0; j < Height; ++j) {
				var center = new Vector2Int(
					rng.RandiRange(GridSize * i + cBuffer.x, GridSize * (i + 1) - 1 + cBuffer.y),
					rng.RandiRange(GridSize * j + cBuffer.x, GridSize * (j + 1) - 1 + cBuffer.y));
				// Switch region on or off -------
				Region newRegion;
				if (i == 0 || j == 0 || i == Width - 1 || j == Height - 1) 
					newRegion = new Region(center, 0);
				else  
					newRegion = new Region(center, (rng.Randf() > 0.3f) ? 1 : 0);
				// -------------------------------
				gridMap.AddRegion(newRegion);
									   
				/// Set random parameters
				randData.Add(newRegion, 
					         new RegRandData() {
					         	color = new Color(rng.Randf(), rng.Randf(), rng.Randf()),
					         	growthSlow = rng.RandfRange(0, 0.8f)});
			}
		}
	}

	public void Initialize() {
		Success = true;
		CreateRegionCenters();
		SetupFill();
	}

	private void SetupFill() { 
		visited = new HashSet<Vector2Int>();
		// Set center tiles.
		foreach (var r in gridMap.GetRegions()) {
			r.tiles.Clear();
			r.Neighbors.Clear();
			r.tiles.Add(r.center);
			visited.Add(r.center);
			AddNeighbors(visited, r, r.center);
		}
	}

	public GridMap Generate() {
		Initialize();
		while (!Step()) {
			Step();
		}
		return gridMap;
	}

	public bool Step() {
		if (visited.Count >= GridSize * GridSize * Width * Height) {
			if (!adjacenciesSet) {
				GD.Print("Recording adjacent tiles");
				SetAdjacencies();
				adjacenciesSet = true;
				GD.Print("........................Done!");

				GD.Print("Ensuring connected landmass");
				EnsureConnectedGraph();
				GD.Print("........................Done!");

				GD.Print("Building Roads...");
				if (GenerateRoads()) 
					GD.Print("........................Done!");
				else {
					GD.PrintErr("ERR: Road generation failed. Stopping.");
					return true;
				}

				GD.Print("Generating border tiles");
				GenerateBorders();
				GD.Print("........................Done!");

				GD.Print("MAP GENERATION COMPLETE:");
				GD.Print(String.Format("Num. Regions: {0}", gridMap.NumActiveRegions));
			}
			return true;
		}
		foreach (var r in gridMap.GetRegions()) {
			PickTile(visited, r);
		}
		return false;
	}

	private void AddNeighbors(HashSet<Vector2Int> visited, Region region, Vector2Int pos) { 
		foreach (var n in gridMap.GetNeighbors(pos.x, pos.y)) {
			if (!visited.Contains(n) && 
				n.x >= 0 && n.y >= 0 && 
				n.x < GridSize * Width && n.y < GridSize * Height) {
				region.Neighbors.Add(n);
			}
			else if (visited.Contains(n) && !region.tiles.Contains(n)){
				region.Adjtiles.Add(n);
			}
		}
	}
	// ^

	private bool PickTile(HashSet<Vector2Int> visited, Region region) {
		if (rng.Randf() < randData[region].growthSlow) return true;
		if (region.Neighbors.IsEmpty()) return false;
		Vector2Int pos = region.Neighbors.Pop();
		while (!region.Neighbors.IsEmpty() && visited.Contains(pos)) {
			if (!region.Adjtiles.Contains(pos) && !region.tiles.Contains(pos)) {
				region.Adjtiles.Add(pos);
			}
			pos = region.Neighbors.Pop();
		}
		if (region.Neighbors.IsEmpty()) return false;
		region.tiles.Add(pos);
		visited.Add(pos);
		AddNeighbors(visited, region, pos);
		return true;
	}

	private void SetAdjacencies() { 
		foreach (Region r in gridMap.GetRegions()) { 
			foreach (Vector2Int v in r.Adjtiles) {
				Region adj = gridMap.GetRegionWithTile(v.x, v.y);
				if (adj != null) {
					r.adjacent.Add(adj);
				}
			}
		}
	}

	private void EnsureConnectedGraph() {
		// Kruskals
		// Generate edges
		var subgraphs = new HashSet<HashSet<Region>>();
		// Build subgraph sets
		foreach (var r in gridMap.GetRegions()) {
			if (r.type != 0)
				subgraphs.Add(new HashSet<Region>() { r });
		}
		// Build edge set
		var edgeSet = new HashSet<Edge<Region>>();
		var edges = new List<Edge<Region>>();
		foreach (var r in gridMap.GetRegions()) { 
			foreach (var adj in r.adjacent) {
				if (adj.type == 0 || r.type == 0) continue;
				float dist = (r.center - adj.center).Magnitude();
				// Set "on" region dist values to the minimum cost 
				// so they're added first
				if (adj.type != 0 && r.type != 0) dist = 0;
				var candidate = new Edge<Region>(r, adj, dist);
				if (!edgeSet.Contains(candidate)) {
					edgeSet.Add(candidate);
				}
			}
		}
		// Sort edge set
		edges = new List<Edge<Region>>(edgeSet);
		edges.Sort((x1, x2) => x1.value.CompareTo(x2.value));
		List<Edge<Region>> newEdges = new List<Edge<Region>>();
		foreach (var e in edges) {
			// Check if we're done
			if (subgraphs.Count == 1) {
				GD.Print("Success!");
				break;
			}
			HashSet<Region> a = null;
			HashSet<Region> b = null;
			foreach (var g in subgraphs) { 
				if (g.Contains(e.a)) {
					a = g;
				}
				if (g.Contains(e.b)) {
					b = g;
				}
				if (a != null && b != null) {
					break;
				}
			}
			if (a == null && b == null) {
				subgraphs.Add(new HashSet<Region>() { e.a, e.b });
			}
			else if (a == null) {
				b.Add(e.a);
			}
			else if (b == null) { 
				a.Add(e.b);
			}

			if (a != b) {
				subgraphs.Remove(b);
				a.UnionWith(b);
				newEdges.Add(e);
			}
		}
		List<HashSet<Region>> graphs = new List<HashSet<Region>>(subgraphs);
		graphs.Sort((x1, x2) => x1.Count.CompareTo(x2.Count));
		HashSet<Region> mGraph = graphs[graphs.Count - 1];

		// Turn regions on if needed
		foreach (var r in gridMap.GetRegions()) { 
			if (!mGraph.Contains(r)) {
				r.type = 0;
			}
		}
	}

	public void GenerateBorderOutline(Region r) {
		var pb = new PathBuilder();
		foreach (var t in r.tiles) {
			var left = new Vector2Int(t.x - 1, t.y); // left
			var right = new Vector2Int(t.x + 1, t.y);
			var up = new Vector2Int(t.x, t.y + 1);
			var down = new Vector2Int(t.x, t.y - 1);
			/*
			+*+    +   +   +
			*x* -> +  [lu][ru] -> convert to float offset to render
			+*+    +  [ld][rd]
			*/
			var lu = new Vector2Int(t.x, t.y); // left
			var ru = new Vector2Int(t.x + 1, t.y);
			var rd = new Vector2Int(t.x + 1, t.y - 1);
			var ld = new Vector2Int(t.x, t.y - 1);
			if (!gridMap.TileIsInRegion(r, left.x, left.y)) 
				pb.Add(ld, lu);
			if (!gridMap.TileIsInRegion(r, right.x, right.y)) 
				pb.Add(ru, rd);
			if (!gridMap.TileIsInRegion(r, up.x, up.y)) 
				pb.Add(lu, ru);
			if (!gridMap.TileIsInRegion(r, down.x, down.y)) 
				pb.Add(rd, ld);
		}
		r.borders = new List<(Vector2Int, Vector2Int)>(pb.BuildPath());
	}

	public void GenerateBorders() {
		gridMap.ActivateAllRegions();
		foreach (var r in gridMap.GetRegions()) {
			foreach (var t in r.Adjtiles) {
				if (!gridMap.Get(t.x, t.y).hasRoad) {
					gridMap.Get(t.x, t.y).open = false;
				}
				//if (!gridMap.TileIsInRegion(t.x + 1))
			}
			GenerateBorderOutline(r);
		}
		// Ensure edges of map are closed.
		for (int x = 0; x < Width * GridSize; ++x) {
			gridMap.Get(x, 0).open = false;
			gridMap.Get(x, (Height * GridSize) - 1).open = false;
		}
		for (int y = 0; y < Height * GridSize; ++y) {
			gridMap.Get(0, y).open = false;
			gridMap.Get((Width * GridSize) - 1, y).open = false;
		}
	}

	public bool GenerateRoads() {
		var edgeList = new HashSet<Edge<Region>>();
		foreach (var r in gridMap.GetRegions()) {
			if (r.type == 0) continue;
			foreach (var adj in r.adjacent) { 
				if (adj.type == 0) continue;
				var candidate = new Edge<Region>(r, adj, 0);
				if (!edgeList.Contains(candidate)) {
					edgeList.Add(candidate);
				}
			}
		}

		foreach (var edge in edgeList) {
			if (!SetRoad(edge)) {
				return false;
			}
		}
		return true;
	}

	private void RejectMap() {
		Success = false;
	}

	private bool SetRoad(Edge<Region> edge) {
		Vector2Int start = edge.a.center;
		Vector2Int end = edge.b.center;
		gridMap.SetActiveRegions(edge.a, edge.b);
		List<Vector2Int> road = null;

		try {
			road = pathFinder.FindPath(start, end);
		}
		catch {
			return false;	
		}

		foreach (var t in road) {
			gridMap.Get(t.x, t.y).hasRoad = true;
		}
		if (road.Count == 0) GD.Print("Failed to find road.");
		if (road.Count > 0) {
			edge.a.roadPaths.Add(road);
		}
		return true;
	}


	// Vector conversions
	// =========================================================================
	public Vector2 GridToWorld(Vector2Int pos) {
		return new Vector2(WorldScale * (pos.x - (float)offset.x), 
						   -WorldScale * (pos.y - (float)offset.y));
	}
}

public class Region {
	public Vector2Int position;
	public HashSet<Vector2Int> tiles = new HashSet<Vector2Int>();
	public Vector2Int center;
	public Vector2Int index;
	public RandList<Vector2Int> Neighbors = new RandList<Vector2Int>();
	public HashSet<Vector2Int> Adjtiles = new HashSet<Vector2Int>();
	public int type;
	public HashSet<Region> adjacent = new HashSet<Region>();
	public List<(Vector2Int, Vector2Int)> roads = new List<(Vector2Int, Vector2Int)>();
	public HashSet<Vector2Int> roadTiles = new HashSet<Vector2Int>();
	public List<List<Vector2Int>> roadPaths = new List<List<Vector2Int>>();
	public HashSet<Vector2Int> borderTiles = new HashSet<Vector2Int>();
	public bool visible;
	public List<(Vector2Int, Vector2Int)> borders; 

	public Region() { 
	}
	public Region(Vector2Int index) {
		this.index = index;
		this.position = index;
	}

	public Region(Vector2Int index, Vector2Int center) {
		this.index = index;
		this.center = center;
	}

	public Region(Vector2Int center, int type) {
		this.center = center;
		this.type = type;
	}

}