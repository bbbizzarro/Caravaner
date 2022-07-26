using Godot;
using System;
using System.Collections.Generic;

public class World :  Node2D, ISavable {
	[Export]
	private int height = 10;
	[Export]
	private int width = 10;
	private int scale = 64;
	[Export]
	private int radius = 2;
	Tile[,] tiles;

	PackedScene tileScene = (PackedScene)ResourceLoader.Load("res://Tile.tscn");
	private HashSet<Tile> renderedTiles;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
	}

	public void Generate() { 
		renderedTiles = new HashSet<Tile>();
		GenerateMap();
		//DrawRadius(width / 2, height / 2, radius);
		DrawRadius(0, 0, radius);
		//DrawMap();
	}

	public override void _Process(float delta) {
		if (Input.IsActionJustPressed("ui_select")) {
			radius += 1;
			//DrawRadius(width / 2, height / 2, radius);
			//DrawRadius(0, 0, radius);
		}
		if (Input.IsActionJustPressed("ui_click")) {
			GridPos pos = WorldToGrid(GetGlobalMousePosition());
			DrawRadius(pos.x, pos.y, radius);
		}
	}

	private struct GridPos {
		public int x;
		public int y;
		public GridPos(int x, int y) {
			this.x = x;
			this.y = y;
		}
	}

	private GridPos WorldToGrid(Vector2 pos) {
		return new GridPos(Mathf.RoundToInt(pos.x / scale), - Mathf.RoundToInt(pos.y / scale));
	}

	// Might be better to create dictionary of data to instantiate with.
	private void GenerateMap() {
		tiles = new Tile[width, height];
		for (int x = 0; x < width; ++x) {
			for (int y = 0; y < height; ++y) {
				tiles[x, y] = new Tile(x * scale, y * scale, false);
			}
		}
	}

	private void DrawRadius(int centX, int centY, int radius) {
		var rng = new RandomNumberGenerator();
		rng.Randomize();
		for (int x = Mathf.Max(0, centX - radius); x <= Mathf.Min(width, centX + radius); ++x) {
			for (int y = Mathf.Max(0, centY - radius); y <= Mathf.Min(height, centY + radius); ++y) {
				float length = Mathf.Sqrt(Mathf.Pow(centX - x, 2) + Mathf.Pow(centY - y, 2));
				if (length <= radius) {
					Tile tile = GetTile(x, y);
					TileRenderer tileRen = DrawTile(tile);
					if (tileRen != null) {
						tileRen.SetSprite(rng.RandiRange(0, 3));
					}
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

	private TileRenderer DrawTile(Tile tile) {
		if (tile == null || renderedTiles.Contains(tile)) {
			return null;
		}
		var newTileObject = (TileRenderer)tileScene.Instance();
		AddChild(newTileObject);
		newTileObject.Position = new Vector2(tile.xPos, -tile.yPos);
		renderedTiles.Add(tile);
		return newTileObject;
	}

	private void DrawMap() {
		foreach (Tile tile in tiles) {
			DrawTile(tile);
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

		return new Godot.Collections.Dictionary<string, object>() {
			{ "Filename", Filename },
			{ "Parent", GetParent().GetPath()},
			{ "Map", tileData}
		};
	}

	public void Load(Godot.Collections.Dictionary<string, object> data) {
		var test = data["Map"];
		GD.Print(test);
	}
}

public class Tile : ISavable {
	public int xPos;
	public int yPos;
	public bool visible = false;
	public Tile(int xPos, int yPos, bool visible) {
		this.xPos = xPos;
		this.yPos = yPos;
		this.visible = visible;
	}


	public void Load(Godot.Collections.Dictionary<string, object> data) {
		xPos = (int)data["PosX"];
		yPos = (int)data["PosY"];
		visible = (bool)data["visible"];
	}

	public Godot.Collections.Dictionary<string, object> Save() {
		return new Godot.Collections.Dictionary<string, object>() {
			{ "PosX", xPos },
			{ "PosY", yPos },
			{ "visible", visible }
		};
	}

	public void SetVisible(bool isVisible) {
		visible = isVisible;
	}

}
