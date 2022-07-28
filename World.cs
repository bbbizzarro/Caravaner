using Godot;
using System;
using Caravaner;
using System.Collections.Generic;

public class World :  Node2D, ISavable {
	[Export, SerializeField] private int height = 10;
	[Export, SerializeField] private int width = 10;
	[Export, SerializeField] private int scale = 64;
	[Export, SerializeField] private int radius = 2;

	Tile[,] tiles;
	PackedScene tileScene = (PackedScene)ResourceLoader.Load("res://Tile.tscn");
	private HashSet<Tile> renderedTiles;

	public void Generate() {
		Initialize();
	}

	private void Initialize() { 
		renderedTiles = new HashSet<Tile>();
		GenerateMap();
	}

	public override void _Process(float delta) {
		if (Input.IsActionJustPressed("ui_select")) {
			radius += 1;
		}
		if (Input.IsActionJustPressed("ui_click")) {
			Vector2Int pos = WorldToGrid(GetGlobalMousePosition());
			DrawRadius(pos.x, pos.y, radius);
		}
	}

	private Vector2Int WorldToGrid(Vector2 pos) {
		return new Vector2Int(Mathf.RoundToInt(pos.x / scale), - Mathf.RoundToInt(pos.y / scale));
	}

	private Vector2 GridToWorld(Vector2Int pos) {
		return new Vector2(pos.x * scale, -pos.y * scale);
	}

	// Might be better to create dictionary of data to instantiate with.
	private void GenerateMap() {
		tiles = new Tile[width, height];
		var rng = new RandomNumberGenerator();
		rng.Randomize();
		for (int x = 0; x < width; ++x) {
			for (int y = 0; y < height; ++y) {
				tiles[x, y] = new Tile(rng.RandiRange(0, 3), false);
			}
		}
	}

	private void DrawRadius(int centX, int centY, int radius) {
		for (int x = Mathf.Max(0, centX - radius); x <= Mathf.Min(width, centX + radius); ++x) {
			for (int y = Mathf.Max(0, centY - radius); y <= Mathf.Min(height, centY + radius); ++y) {
				float length = Mathf.Sqrt(Mathf.Pow(centX - x, 2) + Mathf.Pow(centY - y, 2));
				if (length <= radius) {
					DrawTile(x, y, GetTile(x,y));
				}
			}
		}
	}

	public Tile GetTile(int x, int y) {
		if (tiles == default
			|| x < 0 || x >= width
			|| y < 0 || y >= height) {
			return null;
		}
		return tiles[x, y];
	}

	private TileRenderer DrawTile(int x, int y, Tile tile) {
		if (tile == null || renderedTiles.Contains(tile)) {
			return null;
		}
		tile.visible = true;
		TileRenderer newTileObject = (TileRenderer)tileScene.Instance();
		AddChild(newTileObject);
		// Initialize tile renderer
		newTileObject.Initialize(x * scale, -y * scale, tile.type);
		renderedTiles.Add(tile);
		return newTileObject;
	}

	private void DrawMap() {
		foreach (Tile tile in tiles) {
			//DrawTile(tile);
			//newTileObject.Name = Guid.NewGuid().ToString();
			//newTileObject.Set("Position", new Vector2(tile.xPos, tile.yPos));
			//newTileObject.Set("Position", new Vector2(500, 500));
			//GD.Print(String.Format("Initializing tile at ('{0}', '{1}').", tile.xPos, tile.yPos));
		}
	}

	public Godot.Collections.Dictionary<string, object> Save() {
		List<string> tileData = new List<string>();
		foreach (Tile tile in tiles) {
			tileData.Add(JSON.Print(tile.Save()));
		}
		var data = JSONUtils.SerializeNode(this);
		data["map"] = tileData;
		return data;
	}

	public void Load(Godot.Collections.Dictionary<string, object> data) {
		width = Mathf.RoundToInt((float)data["width"]);
		height = Mathf.RoundToInt((float)data["height"]);
		scale = Mathf.RoundToInt((float)data["scale"]);
		// When drawing tile set visible and only then.
		Generate();

		int index = 0;
		int x, y;
		foreach (string tile in (Godot.Collections.Array)data["map"]) {
			var tileData = new Godot.Collections.Dictionary<string, object>((Godot.Collections.Dictionary)JSON.Parse(tile).Result);
			x = index / width;
			y = index % width;
			tiles[x, y].Load(tileData);
			if (tiles[x, y].visible) {
				DrawTile(x, y, tiles[x, y]);
			}
			index += 1;
		}
	}
}

public class Tile : ISavable {
	[SerializeField] public bool visible = false;
	[SerializeField] public int type;
	public Tile(int type, bool visible) {
		this.visible = visible;
		this.type = type;
	}

	public void Load(Godot.Collections.Dictionary<string, object> data) {
		visible = (bool)data["visible"];
		type = Mathf.RoundToInt((float)data["type"]);
	}

	public Godot.Collections.Dictionary<string, object> Save() {
		return JSONUtils.Serialize(this);
	}

	public void SetVisible(bool isVisible) {
		visible = isVisible;
	}

}
