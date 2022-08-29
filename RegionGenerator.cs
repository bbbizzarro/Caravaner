using System;
using System.Collections;
using System.Collections.Generic;
using Godot;
using Caravaner;
using System.Linq;

public class RegionGenerator : Node2D {
	// Ought to radomly sort neighbors by desirability according
	// to an input image (NOT SIMPLE NOISE!)
	// Will probably need my PQ

	[Export] float WorldScale = 64f;
	[Export] Color VertexColor = new Color(1, 1, 1, 1);
	HashSet<Region> regions = new HashSet<Region>();
	RandomNumberGenerator rng = new RandomNumberGenerator();
	HashSet<Region> drawn = new HashSet<Region>();
	int Width = 10;
	int Height = 10;
	float WorldWidth;
	float WorldHeight;
	Region[,] map;
	Vector2Int offset;
	int GridSize = 15;
	[Export] public bool Randomize { set { CreateRegionCenters(); } get { return true; } }
	[Export] public bool ShowConnections = true;
	HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
	Timer timer;
	[Export] bool GenerateOn;
	[Export] float AnimationDelay = 0f;
	[Export] bool GridOn;
	OpenSimplexNoise noise = new OpenSimplexNoise();
	List<Color> subregionColors = new List<Color>();
	bool adjacenciesSet = false;
	GridMap gridMap;
	PathFinder pathFinder;

	Vector2Int[] DirArray = {
			new Vector2Int(-1, 0),
			new Vector2Int(0, -1),
			new Vector2Int(1, 0),
			new Vector2Int(0, 1)
	};

	Dictionary<Direction, Vector2Int> dirs = new Dictionary<Direction, Vector2Int>(){
		{Direction.W, new Vector2Int(-1, 0) },
		{Direction.S, new Vector2Int(0, -1) },
		{Direction.E, new Vector2Int(1, 0) },
		{Direction.N, new Vector2Int(0, 1) }
	};

	private List<Region> GetNeighbors(int x, int y) {
		var neighbors = new List<Region>();
		foreach (Vector2Int dir in dirs.Values) {
			Vector2Int pos = new Vector2Int(x + dir.x, y + dir.y);
			if (pos.x >= 0 && pos.y >= 0 && pos.x < Width && pos.y < Height) {
				neighbors.Add(map[pos.x, pos.y]);
			}
		}
		return neighbors;

	}

	public int NumbActiveRegions() {
		int total = 0;
		foreach (var r in map) {
			total += (r.type == 0) ? 0 : 1;
		}
		return total;
	}

	// Generation entry point
	private void CreateRegionCenters() {
		WorldWidth = Width * WorldScale;
		WorldHeight = Height * WorldScale;
		offset = new Vector2Int(Width / 2, Height / 2);
		map = new Region[Width, Height];
		gridMap = new GridMap(Width * GridSize, Height * GridSize);
		pathFinder = new PathFinder(100, gridMap);
		adjacenciesSet = false;
		Vector2Int cBuffer = new Vector2Int(1, -1);
		GD.Print(String.Format("Creating new map:"));
		GD.Print(String.Format("Grid Width: {0} Grid Height: {1}", 
			Width * GridSize, Height * GridSize));
		for (int i = 0; i < Width; ++i) { 
			for (int j = 0; j < Height; ++j) {
				Vector2Int index = new Vector2Int(i, j);
				var center = new Vector2Int(
					rng.RandiRange(GridSize * i + cBuffer.x, GridSize * (i + 1) - 1 + cBuffer.y),
					rng.RandiRange(GridSize * j + cBuffer.x, GridSize * (j + 1) - 1 + cBuffer.y));
				//center = IndexToGrid(index);
				map[i, j] = new Region(index, center);

				// Switch region on or off -------
				if (i == 0 || j == 0 || i == Width - 1 || j == Height - 1) {
					map[i, j].type = 0;
				}
				else { 
					map[i, j].type = (rng.Randf() > 0.3f) ? 1 : 0;
				}
				// -------------------------------
									   
				map[i, j].SetDirs((j == Height - 1) ? 0 : 1, 
								  (j == 0) ? 0 : 1, 
								  (i == Width - 1) ? 0 : 1, 
								  (i == 0) ? 0 : 1);
				/// Set random variables
				Color randColor = new Color(rng.Randf(), rng.Randf(), rng.Randf());
				map[i, j].color = randColor;
				map[i, j].growthSlow = rng.RandfRange(0, 0.8f);
			}
		}
		gridMap.SetRegions(map.Cast<Region>());
	}

	private void SetupFill() { 
		visited = new HashSet<Vector2Int>();
		Stack<Region> activeRegions = new Stack<Region>();
		// Set center tiles.
		for (int i = 0; i < Width; ++i) {
			for (int j = 0; j < Height; ++j) {
				map[i, j].tiles.Clear();
				map[i, j].Neighbors.Clear();
				map[i, j].tiles.Add(map[i, j].center);
				visited.Add(map[i, j].center);
				activeRegions.Push(map[i, j]);
				AddNeighbors(visited, map[i, j], map[i, j].center);
			}
		}

	}

	private void FillRegions() {
		if (visited.Count >= GridSize * GridSize * Width * Height) {
			GenerateOn = false;
			if (!adjacenciesSet) {
				GD.Print("Recording adjacent tiles");
				SetAdjacencies();
				adjacenciesSet = true;
				GD.Print("........................Done!");

				GD.Print("Ensuring connected landmass");
				EnsureConnectedGraph();
				GD.Print("........................Done!");

				GD.Print("Building Roads...");
				GenerateRoads();
				GD.Print("........................Done!");

				GD.Print("Generating border tiles");
				GenerateBorders();
				GD.Print("........................Done!");

				GD.Print("MAP GENERATION COMPLETE:");
				GD.Print(String.Format("Num. Regions: {0}", gridMap.NumActiveRegions));
			}
			return;
		}
		for (int i = 0; i < Width; ++i) {
			for (int j = 0; j < Height; ++j) {
				PickTile(visited, map[i, j]);
			}
		}
	}

	private void AddNeighbors(HashSet<Vector2Int> visited, Region region, Vector2Int pos) { 
		foreach (Vector2Int dir in DirArray) {
			Vector2Int n = new Vector2Int(pos.x + dir.x, pos.y + dir.y);
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

	private bool PickTile(HashSet<Vector2Int> visited, Region region) {
		if (rng.Randf() < region.growthSlow) return true;
		if (region.Neighbors.IsEmpty()) return false;
		Vector2Int pos = region.Neighbors.Pop();
		while (!region.Neighbors.IsEmpty() && visited.Contains(pos)) {
			if (!region.Adjtiles.Contains(pos) && !region.tiles.Contains(pos))
				region.Adjtiles.Add(pos);
			pos = region.Neighbors.Pop();
		}
		if (region.Neighbors.IsEmpty()) return false;
		region.tiles.Add(pos);
		visited.Add(pos);
		AddNeighbors(visited, region, pos);
		return true;
	}

	private Region GetParentRegion(Vector2Int v) { 
		foreach (Region r in map) { 
			if (r.tiles.Contains(v)) {
				return r;
			}
		}
		return null;
	}

	private void SetAdjacencies() { 
		foreach (Region r in map) { 
			foreach (Vector2Int v in r.Adjtiles) {
				Region adj = GetParentRegion(v);
				if (adj != null) {
					r.adjacent.Add(adj);
				}
			}
		}
		//!!!!!!!
		//GenerateRoads();
	}

	private void ClearRegionTiles() {
		for (int i = 0; i < Width; ++i) {
			for (int j = 0; j < Height; ++j) {
				map[i, j].Neighbors.Clear();
				map[i, j].tiles.Clear();
			}
		}
	}

	private void EnsureConnectedGraph() {
		// Kruskals
		// Generate edges
		var subgraphs = new HashSet<HashSet<Region>>();
		// Build subgraph sets
		foreach (var r in map) {
			if (r.type != 0)
				subgraphs.Add(new HashSet<Region>() { r });
		}
		// Build edge set
		var edgeSet = new HashSet<MapEdge>();
		var edges = new List<MapEdge>();
		foreach (var r in map) { 
			foreach (var adj in r.adjacent) {
				if (adj.type == 0 || r.type == 0) continue;
				float dist = (r.center - adj.center).Magnitude();
				// Set "on" region dist values to the minimum cost 
				// so they're added first
				if (adj.type != 0 && r.type != 0) dist = 0;
				var candidate = new MapEdge(r, adj, dist);
				if (!edgeSet.Contains(candidate)) {
					edgeSet.Add(candidate);
				}
			}
		}
		// Sort edge set
		edges = new List<MapEdge>(edgeSet);
		edges.Sort((x1, x2) => x1.value.CompareTo(x2.value));
		List<MapEdge> newEdges = new List<MapEdge>();
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

		//Stack<HashSet<Region>> toResolve = new Stack<HashSet<Region>>(graphs);
		//var curr = toResolve.Pop();
		//HashSet<Region> rem;
		//while (toResolve.Count > 0) {
		//	rem = toResolve.Pop();
		//	var candidates = new List<(Region, Region, float)>();
		//	// Find nearest
		//	foreach (var va in curr) { 
		//		foreach (var vb in rem) {
		//			candidates.Add((va, vb, (va.center - vb.center).Magnitude()));
		//		}
		//	}
		//	candidates.Sort((x1, x2) => x1.Item3.CompareTo(x2.Item3));
		//}

		// Turn regions on if needed
		foreach (var r in map) { 
			if (!mGraph.Contains(r)) {
				r.type = 0;
			}
		}
		//foreach (var e in newEdges) { 
		//	if (e.a.type == 0) {
		//		e.a.type = 1;
		//	}
		//	if (e.b.type == 0) {
		//		e.b.type = 1;
		//	}
		//}
	}

	public void GenerateBorders() {
		gridMap.ActivateAllRegions();
		foreach (var r in map) {
			foreach (var t in r.Adjtiles) {
				if (!gridMap.Get(t.x, t.y).hasRoad) {
					gridMap.Get(t.x, t.y).open = false;
				}
			}
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

	public void GenerateRoads() {
		var edgeList = new HashSet<MapEdge>();
		var visited = new HashSet<MapEdge>();
		foreach (var r in map) {
			if (r.type == 0) continue;
			foreach (var adj in r.adjacent) { 
				if (adj.type == 0) continue;
				var candidate = new MapEdge(r, adj, 0);
				if (!edgeList.Contains(candidate)) {
					edgeList.Add(candidate);
				}
			}
		}

		foreach (var edge in edgeList) {
			SetRoad(edge);
		}
	}

	// Maybe use A* and a noise map to modify cost function
	// Also create a 2d array for the grid tiles from region tiles
	// Regions can reference this 2d array to get the Tile struct data
	private void SetRoad(MapEdge edge) {
		Vector2Int start = edge.a.center;
		Vector2Int end = edge.b.center;
		gridMap.SetActiveRegions(edge.a, edge.b);
		var road = pathFinder.FindPath(start, end);
		foreach (var t in road) {
			gridMap.Get(t.x, t.y).hasRoad = true;
		}
		if (road.Count == 0) GD.Print("Failed to find road.");
		if (road.Count > 0) {
			edge.a.roadPaths.Add(road);
		}
	}

	private struct MapEdge {
		public Region a;
		public Region b;
		public float value;
		public MapEdge(Region a, Region b, float value) {
			this.a = a;
			this.b = b;
			this.value = value;
		}

		public override bool Equals(object obj) {
			return obj is MapEdge edge && (MapEdge)obj == this;
		}

		public override int GetHashCode() {
			int hashCode = 2118541809;
			hashCode = hashCode * -1521134295 + (EqualityComparer<Region>.Default.GetHashCode(a) + 
												 EqualityComparer<Region>.Default.GetHashCode(b));
			return hashCode;
		}

		public static bool operator==(MapEdge u, MapEdge v) {
			return (u.a == v.a && u.b == v.b) || (u.a == v.b && u.b == v.a);
		}
		public static bool operator!=(MapEdge u, MapEdge v) {
			return (u.a != v.a || u.b != v.b) && (u.a != v.b || u.b != v.a);
		}
	}

	private struct Edge<T, V> {
		public T a;
		public T b;
		public V value;
		public Edge(T a, T b, V value) {
			this.a = a;
			this.b = b;
			this.value = value;
		}
	}

	public override void _Ready() {
		rng.Randomize();
		timer = (Timer)GetNode("Timer");
		timer.Connect("timeout", this, nameof(AnimateBuildRegions));

		noise.Octaves = 4;
		noise.Period = 20f;
		noise.Persistence = 0.8f;

		CreateRegionCenters();
		SetupFill();
		FillRegions();
	}

	public void AnimateBuildRegions() { 
		FillRegions();
		Update();
	}

	public override void _Process(float delta) {
		if (Input.IsActionJustPressed("interact")) {
			CreateRegionCenters();
			SetupFill();
			FillRegions();
			GD.Print("Randomizing!");
			Update();
		}
		if (Input.IsActionJustPressed("interact2")) {
			ShowConnections = !ShowConnections;
			Update();
		}
		if (Input.IsActionJustPressed("ui_select")) {
			GenerateOn = !GenerateOn;
			Update();
		}
		if (GenerateOn) {
			FillRegions();
			Update();
			timer.Start(AnimationDelay);
		}
		else {
			timer.Stop();
		}
	}

	// Vector conversions
	// =========================================================================
	private Vector2Int IndexToGrid(Vector2Int index) {
		return new Vector2Int(GridSize * index.x, GridSize * index.y);
	}
	private Vector2 IndexToWorld(Vector2Int pos) { 
		return new Vector2(WorldScale * (GridSize * pos.x - (float)offset.x), 
						   -WorldScale * (GridSize * pos.y - (float)offset.y));
	}

	private Vector2 GridToWorld(Vector2Int pos) {
		return new Vector2(WorldScale * (pos.x - (float)offset.x), 
						   -WorldScale * (pos.y - (float)offset.y));
	}

	// DRAWING
	// ========================================================================= 
	public override void _Draw() {
		DrawGrid();
		for (int i = 0; i < Width; ++i) { 
			for (int j = 0; j < Height; ++j) {
				DrawRegion(map[i, j]);
			}
		}
		if (ShowConnections) { 
			foreach (Region r in map) { 
				DrawConnections(r);
			}
		}
		foreach (Region r in map) {
			DrawRoads(r);
		}
		if (adjacenciesSet) {
			gridMap.ActivateAllRegions();
			foreach (Region r in map) {
				DrawBorders(r);
			}
		}
	}

	private void DrawGrid() {
		Color gray = new Color(0.5f, 0.5f, 0.5f);
		for (int i = -1; i <= Width; ++i) {
			DrawLine(IndexToWorld(new Vector2Int(i, -1)),
					 IndexToWorld(new Vector2Int(i, Height)),
					 gray);
		}
		for (int i = -1; i <= Height; ++i) {
			DrawLine(IndexToWorld(new Vector2Int(-1, i)),
					 IndexToWorld(new Vector2Int(Width, i)),
					 gray);
		}
	}

	private void DrawBorders(Region r) {
		Color c = (r.type == 0) ? new Color(0, 0, 0) : r.color;
		foreach (var t in r.tiles) {
			if (!gridMap.IsOpen(t.x, t.y)) {
				Vector2 pos = GridToWorld(t);
				DrawRect(new Rect2(new Vector2(pos.x - WorldScale/2f, pos.y - WorldScale/2f), 
								   new Vector2(WorldScale, WorldScale)), 
								   c * 0.5f);
			}
		}
	}

	private void DrawRoads(Region r) { 
		foreach (var road in r.roadPaths) {
			if (road.Count < 2) continue;
			for (int i = 0; i < road.Count - 1; ++i) {
				DrawLine(GridToWorld(road[i]),
						 GridToWorld(road[i + 1]),
						 new Color(0, 0, 0));
			}
		}

		/*
		foreach (var road in r.roads) {
			DrawLine(GridToWorld(road.Item1),
					 GridToWorld(road.Item2),
					 new Color(0, 0, 0));
		}
		*/
	}

	private void DrawRegion(Region r) {
		if (drawn.Contains(r)) return;
		//Color randColor = new Color(rng.Randf(), rng.Randf(), rng.Randf());
		Color c = (r.type == 0) ? new Color(0, 0, 0) : r.color;
		foreach (Vector2Int tile in r.tiles) { 
			Color dc = c;
			// Noise coloring
			//float a = noise.GetNoise2d(tile.x, tile.y);
			//c = new Color(a, a, a);
			Vector2 pos = GridToWorld(tile);
			//if (!gridMap.IsOpen(tile.x, tile.y)) {
			//	dc *= 0.5f;
			//}
			DrawRect(new Rect2(new Vector2(pos.x - WorldScale/2f, pos.y - WorldScale/2f), 
							   new Vector2(WorldScale, WorldScale)), 
							   dc);
		}
	}

	private void DrawConnections(Region r) {
		Color white = new Color(1, 1, 1);
		Color black = new Color(0, 0, 0);
		Color gray = new Color(1f, 1f, 1f, 0.1f);
		//if (r.type == 0) return;

		//foreach (Direction dir in dirs.Keys) {
		//	Vector2Int vDir = dirs[dir];
		//	Vector2Int pos = new Vector2Int(r.index.x + vDir.x, 
		//									r.index.y + vDir.y);
		//	if (pos.x >= 0 && pos.y >= 0 && 
		//		pos.x < Width && pos.y < Height) {
		//		if (map[pos.x, pos.y].type == 0) continue;
		//		DrawLine(GridToWorld(r.center), 
		//				 GridToWorld(map[pos.x, pos.y].center), 
		//				 (r.IsConnected(dir) > 0) ? white : black);
		//	}
		//}
		foreach (Region adj in r.adjacent) {
			DrawLine(GridToWorld(r.center),
					 GridToWorld(adj.center),
					 (adj.type == 0 || r.type == 0) ? gray : white);

		}
	}
}

public struct Edge<T> {
	public readonly T From;
	public readonly T To;
	public Edge(T from, T to) {
		From = from;
		To = to;
	}
}

public interface IGraph<T> {
	bool IsAdjacent(T v, T u);
	bool GetNeighbors(T v);
	void AddVertex(T v);
	void RemoveVertex(T v);
	void AddEdge(T v, T u);
	void RemoveEdge(T v, T u);
}

public class UndirectedGraph<T> : IGraph<T> {
	Dictionary<T, T> edges;
	HashSet<T> vertices;

	public UndirectedGraph() {
		edges = new Dictionary<T, T>();
		vertices = new HashSet<T>();
	}

	public void AddEdge(T v, T u) {
		AddVertex(v);
		AddVertex(u);
		if (!edges.ContainsKey(v))
			edges.Add(v, u);
		if (!edges.ContainsKey(u))
			edges.Add(u, v);
	}

	public void AddVertex(T v) {
		if (!vertices.Contains(v))
			vertices.Add(v);
	}

	public bool GetNeighbors(T v) {
		throw new NotImplementedException();
	}

	public bool IsAdjacent(T v, T u) {
		throw new NotImplementedException();
	}

	public void RemoveEdge(T v, T u) {
		throw new NotImplementedException();
	}

	public void RemoveVertex(T v) {
		throw new NotImplementedException();
	}
}

public class Graph<T> {
	HashSet<Edge<T>> edges;
	HashSet<T> vertices;
}

public class Region {
	public int lowerX;
	public int lowerY;
	public int upperX;
	public int upperY;
	public Vector2Int position;
	public HashSet<Vector2Int> tiles = new HashSet<Vector2Int>();
	protected HashSet<Region> connections = new HashSet<Region>();
	public int subregion;
	public Vector2Int center;
	public Vector2Int index;
	public int N;
	public int S;
	public int E;
	public int W;
	public RandList<Vector2Int> Neighbors = new RandList<Vector2Int>();
	public HashSet<Vector2Int> Adjtiles = new HashSet<Vector2Int>();
	public Color color;
	public float growthSlow;
	public int type;
	public HashSet<Region> adjacent = new HashSet<Region>();
	public List<(Vector2Int, Vector2Int)> roads = new List<(Vector2Int, Vector2Int)>();
	public HashSet<Vector2Int> roadTiles = new HashSet<Vector2Int>();
	public List<List<Vector2Int>> roadPaths = new List<List<Vector2Int>>();

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

	public void SetDirs(int n, int s, int e, int w) {
		N = n; S = s; E = e; W = w;
	}

	public void SetConnection(Direction dir, int value) { 
		switch (dir) {
			case Direction.N: N = value; return;
			case Direction.S: S = value; return;
			case Direction.E: E = value; return;
			case Direction.W: W = value; return;
		}
	}

	public int IsConnected(Direction dir) { 
		switch (dir) {
			case Direction.N: return N;
			case Direction.S: return S;
			case Direction.E: return E;
			case Direction.W: return W;
		}
		return -1;
	}

	public void Connect(Region region) { 
		if (!IsConnectedTo(region)) {
			connections.Add(region);
		}
		if (!region.IsConnectedTo(this)) {
			region.Connect(this);
		}
	}

	public IEnumerable<Region> GetConnections() {
		return connections;
	}

	public bool IsConnectedTo(Region region) {
		return connections.Contains(region);
	}
}
	public enum Direction { 
		N, S, E, W
	}


public class RandList<T> : List<T> {
	RandomNumberGenerator rng = new RandomNumberGenerator();
	public RandList() {
		rng.Randomize();
	}

	public T Pop() {
		int randIndex = rng.RandiRange(0, Count - 1);
		T result = this[randIndex];
		this[randIndex] = this[Count - 1];
		RemoveAt(Count - 1);
		return result;
	}

	public bool IsEmpty() {
		return Count == 0;
	}
}
