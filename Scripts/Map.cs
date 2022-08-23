using System;
using System.Collections.Generic;
using Godot;
using Caravaner;

public class Map : Node {
    TileMap tileMap;
    Dictionary<Vector2Int, int> tiles;
    List<Vector2Int> directions = new List<Vector2Int>() {
            new Vector2Int(0, 1),
            new Vector2Int(0, -1),
            new Vector2Int(-1, 0),
            new Vector2Int(1, 0)
    };
    RandomNumberGenerator rng;

    public override void _Ready() {
        base._Ready();
        tileMap = (TileMap)GetNode("TileMap");
        rng = new RandomNumberGenerator();
        rng.Randomize();
        //PrintOctave(GenerateOctave(10), 10, 2);
        GenerateFromPoint(0, 0, 100);
    }

    public void GenerateFromPoint(int x, int y, int numberOfTiles) {
        var visited = new HashSet<Vector2Int>();
        var neighbors = new List<Vector2Int>() { new Vector2Int(x, y) };
        for (int i = 0; i < numberOfTiles; ++i) {
            int candidateIndex = rng.RandiRange(0, neighbors.Count - 1);
            Vector2Int candidate = neighbors[candidateIndex];
		    neighbors.RemoveAt(candidateIndex);
            visited.Add(candidate);
            tileMap.SetCell(candidate.x, candidate.y, 3);
            foreach (Vector2Int neighbor in GetNeighbors(candidate.x, candidate.y)) { 
                if (!visited.Contains(neighbor)) {
                    neighbors.Add(neighbor);
				}
			}
		}
        foreach (Vector2Int neighbor in neighbors) {
            if (!visited.Contains(neighbor)) {
                GD.Print(neighbor);
                tileMap.SetCell(neighbor.x, neighbor.y, 2);
			}
		}
	}

    public List<Vector2Int> GetNeighbors(int x, int y) {
        var neighbors = new List<Vector2Int>();
        foreach (Vector2Int dir in directions) {
            neighbors.Add(new Vector2Int(x + dir.x, y + dir.y));
		}
        return neighbors;

	}

    public void Set(int x, int y) { 
        tileMap.SetCell(x, y, 2);
	}

    public bool[,] GenerateOctave(int sideLength) {
        RandomNumberGenerator rng = new RandomNumberGenerator();
        rng.Randomize();
        bool[,] octave = new bool[sideLength, sideLength];
        for (int x = 0; x < sideLength; ++x) { 
            for (int y= 0; y < sideLength; ++y) {
                octave[x, y] = rng.Randf() > 0.5f;
			}
		}
        return octave;
	}

    public void PrintOctave(bool[,] map, int sideLength, int divFactor) { 
        for (int x = 0; x < sideLength * divFactor; ++x) { 
            for (int y =0; y < sideLength * divFactor; ++y) { 
                int i = (map[x/divFactor, y/divFactor]) ? 0 : 3;
                GD.Print(i);
                tileMap.SetCell(x-10, y-10, i);
			}
		}
	}

    public void GenerateMap() { 
        tiles = new Dictionary<Vector2Int, int>();
        for (int x = 0; x < 10; ++x) {
            for (int y =0; y < 10; ++y) {
                tiles.Add(new Vector2Int(x, y), (int)TType.Ground);
			}
        }	
	}

    public void RenderMap() { 
        foreach (Vector2Int pos in tiles.Keys) {
            tileMap.SetCell(pos.x, pos.y, tiles[pos]);
		}
	}

    private enum TType { 
        Ground
	}
}
