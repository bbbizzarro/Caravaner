using System;
using System.Collections.Generic;
using Godot;
using Caravaner;

public class WorldRenderer {

    Pool<TileRenderer> tilePool;

    private Dictionary<Vector2Int, List<TileRenderer>> batches;

    public WorldRenderer() {
        tilePool = new Pool<TileRenderer>();
	}

    public void BatchDraw() { 
	}
}
