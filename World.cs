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

	[Signal] delegate void TileChanged(int x, int y);

	public void Generate() {
		Initialize();
	}

	private void Initialize() { 
		GenerateMap();
	}

	public int GetHeight() {
		return height;
	}

	public int GetWidth() {
		return width;
	}

	private Vector2Int WorldToGrid(Vector2 pos) {
		return new Vector2Int(Mathf.RoundToInt((pos.x) / scale), - Mathf.RoundToInt((pos.y) / scale));
	}

	public Tile GetTile(Vector2 pos) {
		Vector2Int gridPos = WorldToGrid(pos);
		return GetTile(gridPos.x, gridPos.y);
	}

	public void UpdateTile(Vector2 pos) {
		Vector2Int gridPos = WorldToGrid(pos);
		Tile tile = GetTile(gridPos.x, gridPos.y);
		if (tile != null) { 
			EmitSignal(nameof(TileChanged), gridPos.x, gridPos.y);
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

	// Might be better to create dictionary of data to instantiate with.
	private void GenerateMap() {
		tiles = new Tile[width, height];
		var rng = new RandomNumberGenerator();
		rng.Randomize();
		for (int x = 0; x < width; ++x) {
			for (int y = 0; y < height; ++y) {
				tiles[x, y] = new Tile(rng.RandiRange(0, 4), false);
				if (tiles[x,y].type == 4) { 
					tiles[x, y].items.Add(rng.RandiRange(0, 4));
				}
			}
		}
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
		// Save map data;
		List<string> tileData = new List<string>();
		foreach (Tile tile in tiles) {
			tileData.Add(JSON.Print(tile.Save()));
		}
		var data = JSONUtils.SerializeNode(this);
		data["map"] = tileData;
		return data;
	}

	public void Load(Godot.Collections.Dictionary<string, object> data) {
		JSONUtils.Deserialize(this, data);

		// When drawing tile set visible and only then.
		Generate();

		// Load map data
		int index = 0;
		int x, y;
		foreach (string tile in (Godot.Collections.Array)data["map"]) {
			var tileData = new Godot.Collections.Dictionary<string, object>((Godot.Collections.Dictionary)JSON.Parse(tile).Result);
			x = index / width;
			y = index % width;
			tiles[x, y].Load(tileData);
			index += 1;
		}
	}
}
