using System;
using System.Collections.Generic;
using Godot;
using Caravaner;

public class WorldRenderer : Node2D {

	PackedScene tileScene = (PackedScene)ResourceLoader.Load("res://Tile.tscn");

	HashSet<Tile> activeTiles;
    Pool<TileRenderer> tilePool;
	private int scale;
	private int radius = 1;
	World world;
	private Vector2Int drawBox = new Vector2Int(500, 500);
	bool[] renderedBatches;

	public override void _Process(float delta) {
		if (Input.IsActionJustPressed("ui_select")) {
			radius += 1;
		}
		if (Input.IsActionJustPressed("ui_click")) {
			Vector2Int pos = WorldToGrid(GetGlobalMousePosition());
			DrawRadius(pos.x, pos.y, radius);
		}
	}

    public void Initialize(int tileCount, int scale, World world) { 
		this.scale = scale;
		this.world = world;
		activeTiles = new HashSet<Tile>();
		int subdivs = 6;
		int sideLength = Mathf.RoundToInt(Mathf.Ceil(Mathf.Sqrt((float)world.GetWidth() * (float)world.GetHeight() / (float)subdivs)));

		// Fill the pool with tile renderers.
        tilePool = new Pool<TileRenderer>();
		for (int i = 0; i < tileCount; ++i) { 
			TileRenderer newTileObject = (TileRenderer)tileScene.Instance();
			newTileObject.Connect("NodeActiveSet", this, "SetNode");
			tilePool.Add(newTileObject);
			AddChild(newTileObject);
		}
		DrawMap();
	}

	public void DrawRadius(Vector2 pos, int radius) {
		Vector2Int gridPos = WorldToGrid(pos);
		DrawRadius(gridPos.x, gridPos.y, radius);
	}

	public void DrawMap() {
		for (int x = 0; x < world.GetWidth(); ++x) {
			for (int y = 0; y < world.GetHeight(); ++y) {
				Tile tile = world.GetTile(x, y);
				if (tile != null && tile.visible) { 
					DrawTile(x, y, world.GetTile(x, y));
				}
			}
		}
	}

	public void SetNode(TileRenderer tileRenderer, bool isActive) { 
		if (isActive) {
			//AddChild(tileRenderer);
		}
		else {
			//RemoveChild(tileRenderer);
		}
	}

	private Vector2Int WorldToGrid(Vector2 pos) {
		return new Vector2Int(Mathf.RoundToInt(pos.x / scale), - Mathf.RoundToInt(pos.y / scale));
	}

	private Vector2 GridToWorld(Vector2Int pos) {
		return new Vector2(pos.x * scale, -pos.y * scale);
	}

	private void DrawRadius(int centX, int centY, int radius) {
		for (int x = Mathf.Max(0, centX - radius); x <= Mathf.Min(world.GetWidth(), centX + radius); ++x) {
			for (int y = Mathf.Max(0, centY - radius); y <= Mathf.Min(world.GetHeight(), centY + radius); ++y) {
				float length = Mathf.Sqrt(Mathf.Pow(centX - x, 2) + Mathf.Pow(centY - y, 2));
				if (length <= radius) {
					DrawTile(x, y, world.GetTile(x, y));
				}
			}
		}
	}

	private void DrawTile(int x, int y, Tile tile) {
		if (tile == null || tilePool.IsEmpty() || activeTiles.Contains(tile)) return;
		tile.visible = true;
		tilePool.Get().Initialize(x * scale, -y * scale, tile.type);
		activeTiles.Add(tile);
	}

	private void BatchDraw(Vector2 center) {
		// Map position to batch.
		// If batch is already being rendered, then skip
		Vector2Int gridPos = WorldToGrid(center);

	}
}
