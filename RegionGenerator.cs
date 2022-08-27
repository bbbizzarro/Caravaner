using System;
using System.Collections;
using System.Collections.Generic;
using Godot;
using Caravaner;
using System.Linq;

public class RegionGenerator : Node2D {

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
	[Export] float AnimationDelay = 0.2f;
	OpenSimplexNoise noise = new OpenSimplexNoise();
	List<Color> subregionColors = new List<Color>();

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

	private void CreateRegionCenters() {
		WorldWidth = Width * WorldScale;
		WorldHeight = Height * WorldScale;
		offset = new Vector2Int(Width / 2, Height / 2);
		map = new Region[Width, Height];
		Vector2Int cBuffer = new Vector2Int(1, -1);
		for (int i = 0; i < Width; ++i) { 
			for (int j = 0; j < Height; ++j) {
				Vector2Int index = new Vector2Int(i, j);
				var center = new Vector2Int(
					rng.RandiRange(GridSize * i + cBuffer.x, GridSize * (i + 1) - 1 + cBuffer.y),
					rng.RandiRange(GridSize * j + cBuffer.x, GridSize * (j + 1) - 1 + cBuffer.y));
				//center = IndexToGrid(index);
				map[i, j] = new Region(index, center);
									   
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
			return;
		}
		for (int i = 0; i < Width; ++i) {
			for (int j = 0; j < Height; ++j) {
				PickTile(visited, map[i, j]);
			}
		}

		/*
		Stack<Region> nextActive = new Stack<Region>();
		while (activeRegions.Count > 0) { 
			while (activeRegions.Count > 0) {
				Region r = activeRegions.Pop();
				if (PickTile(visited, r)) {
					nextActive.Push(r);
				}
			}
			var temp = activeRegions;
			activeRegions = nextActive;
			nextActive = temp;
		}
		*/
	}

	private void AddNeighbors(HashSet<Vector2Int> visited, Region region, Vector2Int pos) { 
		foreach (Vector2Int dir in DirArray) {
			Vector2Int n = new Vector2Int(pos.x + dir.x, pos.y + dir.y);
			if (!visited.Contains(n) && 
				n.x >= 0 && n.y >= 0 && 
				n.x < GridSize * Width && n.y < GridSize * Height) {
				region.Neighbors.Add(n);
			}
		}
	}

	private bool PickTile(HashSet<Vector2Int> visited, Region region) {
		if (rng.Randf() < region.growthSlow) return true;
		if (region.Neighbors.IsEmpty()) return false;
		Vector2Int pos = region.Neighbors.Pop();
		while (!region.Neighbors.IsEmpty() && visited.Contains(pos)) {
			pos = region.Neighbors.Pop();
		}
		if (region.Neighbors.IsEmpty()) return false;
		region.tiles.Add(pos);
		visited.Add(pos);
		AddNeighbors(visited, region, pos);
		return true;
	}

	private void ClearRegionTiles() {
		for (int i = 0; i < Width; ++i) {
			for (int j = 0; j < Height; ++j) {
				map[i, j].Neighbors.Clear();
				map[i, j].tiles.Clear();
			}
		}
	}

	private void CreateMST(int startX, int startY) {
		HashSet<Region> visited = new HashSet<Region>();
		Queue<Region> candidates = new Queue<Region>();
		candidates.Enqueue(map[startX, startY]);
		while (candidates.Count > 0) {
			Region r = candidates.Dequeue();
			foreach (Region n in GetNeighbors(r.index.x, r.index.y)) { 
				if (!visited.Contains(n)) {
					candidates.Enqueue(n);
				}
			}
		}
	}

	public override void _Ready() {
		timer = (Timer)GetNode("Timer");
		timer.Connect("timeout", this, nameof(AnimateBuildRegions));

		noise.Octaves = 4;
		noise.Period = 20f;
		noise.Persistence = 0.8f;

		CreateRegionCenters();
		SetupFill();
		FillRegions();
		rng.Randomize();
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


	private void DrawRegion(Region r) {
		if (drawn.Contains(r)) return;
		//Color randColor = new Color(rng.Randf(), rng.Randf(), rng.Randf());
		foreach (Vector2Int tile in r.tiles) { 
			Vector2 pos = GridToWorld(tile);
			Color c = r.color;
			float a = noise.GetNoise2d(tile.x, tile.y);
			//c = new Color(a, a, a);
			DrawRect(new Rect2(new Vector2(pos.x - WorldScale/2f, pos.y - WorldScale/2f), 
							   new Vector2(WorldScale, WorldScale)), 
							   c);
		}
		if (ShowConnections) { 
			DrawConnections(r);
		}
	}

	private void DrawConnections(Region r) {
		Color white = new Color(1, 1, 1);
		Color black = new Color(0, 0, 0);
		foreach (Direction dir in dirs.Keys) {
			Vector2Int vDir = dirs[dir];
			Vector2Int pos = new Vector2Int(r.index.x + vDir.x, 
											r.index.y + vDir.y);
			if (pos.x >= 0 && pos.y >= 0 && 
				pos.x < Width && pos.y < Height) {
				DrawLine(GridToWorld(r.center), 
						 GridToWorld(map[pos.x, pos.y].center), 
						 (r.IsConnected(dir) > 0) ? white : black);
			}
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
	public Color color;
	public float growthSlow;

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
