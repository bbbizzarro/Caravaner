using System;
using System.Collections;
using System.Collections.Generic;
using Godot;
using Caravaner;

public class RegionGenerator {
    public RegionGenerator() {
    }

    public IEnumerable<WorldTile> Generate(int minX, int minY, int maxX, int maxY) {
        List<WorldTile> tiles = new List<WorldTile>();
        for (int x = minX; x < maxX; ++x) { 
            for (int y = minY; y < maxY; ++y) {
                tiles.Add(new WorldTile(new Vector2Int(x, y), "Land"));
			}
		}
        return tiles;
	}
}

public struct WorldTile {
    public Vector2Int position;
    public string tileName;

    public WorldTile(Vector2Int position, string tileName) {
        this.position = position;
        this.tileName = tileName;
	}
}
