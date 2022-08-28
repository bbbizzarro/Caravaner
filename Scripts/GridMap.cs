using Caravaner;
using Godot;
using System;
using System.Collections.Generic;

public class GridMap : IPathGrid {
    public int Height { private set; get; }
    public int Width {private set; get; }

    private readonly Tile[,] map;
    private readonly HashSet<Region> activeRegions = new HashSet<Region>();
	Vector2Int[] dirs = {new Vector2Int(-1, 0),  new Vector2Int(0, -1),
                         new Vector2Int( 1, 0),  new Vector2Int(0,  1)};

    public GridMap(int width, int height) {
        Height = height;
        Width = width;
        map = InitializeMap(width, height);
    }

    public void SetActiveRegions(IEnumerable<Region> regions) {
        activeRegions.Clear();
        foreach (var r in regions) activeRegions.Add(r);
    }

    public void SetActiveRegions(params Region[] regions) {
        activeRegions.Clear();
        foreach (var r in regions) activeRegions.Add(r);
    }

    public List<Vector2Int> GetOpenNeighbors(int x, int y) {
        var center = new Vector2Int(x, y);
        var neighbors = new List<Vector2Int>();
        foreach (var dir in dirs) {
            var neighbor = center + dir;
            if (IsOpen(neighbor.x, neighbor.y)) {
                neighbors.Add(neighbor);
            }
        }
        return neighbors;
    }

    public bool IsOpen(int x, int y) {
        return x >= 0 && y >= 0 && x < Width && y < Height && 
               InActiveRegions(x, y);
    }

    public bool InActiveRegions(int x, int y) {
        var v = new Vector2Int(x, y);
        foreach (var r in activeRegions) {
            if (r.tiles.Contains(v)) return true;
        }
        return false;
    }

    private Tile[,] InitializeMap(int width, int height) {
        var map = new Tile[width, height];
        for (int x = 0; x < width; ++x) {
            for (int y = 0; y < width; ++y) {
                map[x,y] = new Tile(0, false);
            }
        }
        return map;
    }
}