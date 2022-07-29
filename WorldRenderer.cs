using System;
using System.Collections.Generic;
using Godot;
using Caravaner;

public class WorldRenderer : Node2D {

    Pool<TileRenderer> tilePool;
	PackedScene tileScene = (PackedScene)ResourceLoader.Load("res://Tile.tscn");

	// Each batch is a partition of the world
	// A tile can be visible but not drawn => "visible" is more like "explored"
    private Dictionary<Vector2Int, List<TileRenderer>> batches;
	private Dictionary<Vector2Int, bool> activeBatches;
	int scale;

    public void Initialize(int tileCount, int scale, int height, int width, int numBatches) {
		// Really we need to calculate the viewport size relative to tile size
		// i.e. how many tiles we can see at once.
		this.scale = scale;
        tilePool = new Pool<TileRenderer>();
		for (int i = 0; i < tileCount; ++i) { 
			TileRenderer newTileObject = (TileRenderer)tileScene.Instance();
			tilePool.Add(newTileObject);
			AddChild(newTileObject);
		}
	}

	private Vector2Int GridToBatch(int x, int y) {
		return Vector2Int.Zero;
	}

    public void DrawTile(int x, int y, Tile tile) {

		// Avoid drawing tile from an inactive batch
		if (!activeBatches[GridToBatch(x, y)]) {
			return;
		}
	}

    public void DrawBatch() { 
	}
}
