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
    HashSet<Vector2Int> visited;
    const float Scale = 64f;

    private enum Dir { 
        Up = 0,
        Down = 1,
        Left = 2,
        Right = 3
	}

    /*
    public override void _Ready() {
        base._Ready();
        tileMap = (TileMap)GetNode("TileMap");
        rng = new RandomNumberGenerator();
        rng.Randomize();
        //PrintOctave(GenerateOctave(10), 10, 2);
        PlaceScenes(GenerateFromPoint(0, 0, 100));
        //GenerateFromPoint(-4,0, 100);
        //GenerateNodes(5, 10);
    }
    */

    public void Initialize() { 
        tileMap = (TileMap)GetNode("TileMap");
        rng = new RandomNumberGenerator();
        rng.Randomize();
        //PrintOctave(GenerateOctave(10), 10, 2);
        Vector2Int p1 = new Vector2Int(0, 0);
        Vector2Int p2 = new Vector2Int(-7, 0);
        Vector2Int p3 = new Vector2Int(-3, -7);
        PlaceScenes(GenerateFromPoint(p1.x, p1.y, rng.RandiRange(40, 140)));
        GenerateFromPoint(p2.x,p2.y, rng.RandiRange(40, 140));
        GenerateFromPoint(p3.x,p3.y, rng.RandiRange(40, 140));
        DrawPath(p1, p2);
        DrawPath(p2, p3);
        //GenerateNodes(5, 10);
	}


    public class Region {
        public int lowerX;
        public int lowerY;
        public int upperX;
        public int upperY;
        public HashSet<Vector2Int> tiles;
	}

    public void GenerateRegions(int n, int size) {
        // Create bounding box over current region

	}

    public void PlaceScenes(Region region) {
        var invalidPoints = new HashSet<Vector2Int>();
        PlaceScene(invalidPoints, region, "Exchange");
        PlaceScene(invalidPoints, region, "Mechanist");
        PlaceScene(invalidPoints, region, "ResourcePoint");
        PlaceScene(invalidPoints, region, "ResourcePoint");
        PlaceScene(invalidPoints, region, "CampSite");
	}

    public void PlaceScene(HashSet<Vector2Int> invalidPoints, Region region, string name) { 
        Vector2Int exchange
            = GetRandomPoint(new List<Vector2Int>(region.tiles), invalidPoints);
        invalidPoints.Add(exchange);
        Vector2 v = new Vector2(exchange.x * Scale, exchange.y * Scale);
        Services.Instance.TileInstancer.Spawn(v, name);
	}

    public Vector2Int GetRandomPoint(List<Vector2Int> points, HashSet<Vector2Int> invalidPoints) {
        for (int i = 0; i < 100; ++i) {
            Vector2Int p = points[rng.RandiRange(0, points.Count - 1)];
            if (!invalidPoints.Contains(p) && tileMap.GetCell(p.x, p.y)!= 5) return p;
		}
        GD.PrintErr("Could not get random point.");
        return Vector2Int.Zero;
	}

    public void GenerateNodes(int size, int gridSideLenth) {
        var nodes = new List<Vector2Int>();
        int length = size / 2 * gridSideLenth;
        for (int x = -length; x < length; x+= gridSideLenth) {
            for (int y = -length; y < length; y += gridSideLenth) {
                // Get random point in square.
                Vector2Int randomPoint = new Vector2Int(
                    rng.RandiRange(x, x + gridSideLenth - 1),
                    rng.RandiRange(y, y + gridSideLenth - 1)
                );
                nodes.Add(randomPoint);

                //nodes.Add(new Vector2Int(x * gridSideLenth, y * gridSideLenth));
                //if (x % 2 == 0 || y % 2 == 1) continue; 
                int t;
                if (Math.Abs(x / gridSideLenth) % 2 != Math.Abs(y / gridSideLenth) % 2)
                    t = 1;
                else t = 3;

                for (int xi = x; xi < x + gridSideLenth; ++xi) {
                    for (int yi = y; yi < y + gridSideLenth; ++yi) {
                        tileMap.SetCell(xi, yi, t);
                    }
                }
			}
        }
        foreach (Vector2Int v in nodes) {
            tileMap.SetCell(v.x, v.y, 0);
		}
	}

    public void SetActiveTile(int x, int y) { 
        tileMap.SetCell(x, y, 3);
        foreach (Vector2Int v in GetNeighbors(x, y)) { 
            if (!visited.Contains(v)) { 
                tileMap.SetCell(v.x, v.y, 2);
			}
		}
	}

    public void DrawPath(Vector2Int start, Vector2Int finish) {
        Vector2Int direction = (finish - start).Normalized();
        int maxP = 0;
        while (start != finish && maxP < 100) {
            start += direction;
            tileMap.SetCell(start.x, start.y, 6);
            direction = (finish - start).Normalized();
            if (Mathf.Abs(direction.x) == 1 && Mathf.Abs(direction.y) == 1) {
                int x = Mathf.RoundToInt(rng.Randf());
                int y = (x == 1) ? 0 : 1;
                direction = new Vector2Int(Mathf.Sign(direction.x) * x,
                                           Mathf.Sign(direction.y) * y);
			}
            maxP += 1;
		}
        tileMap.UpdateBitmaskRegion();
	}

    public Region GenerateFromPoint(int x, int y, int numberOfTiles) {
        if (visited == null)
            visited = new HashSet<Vector2Int>();
        if (visited.Contains(new Vector2Int(x, y))) {
            GD.PrintErr("Invalid generation point.");
            return null;
		}
        var neighbors = new List<Vector2Int>() { new Vector2Int(x, y) };
        Region region = new Region() {};
        HashSet<Vector2Int> regionTiles = new HashSet<Vector2Int>();
        for (int i = 0; i < numberOfTiles; ++i) {
            int candidateIndex = rng.RandiRange(0, neighbors.Count - 1);
            Vector2Int candidate = neighbors[candidateIndex];
		    neighbors.RemoveAt(candidateIndex);
            visited.Add(candidate);
            tileMap.SetCell(candidate.x, candidate.y, 4);
            //SetActiveTile(candidate.x, candidate.y);
            regionTiles.Add(candidate);
            foreach (Vector2Int neighbor in GetNeighbors(candidate.x, candidate.y)) { 
                if (!visited.Contains(neighbor)) {
                    neighbors.Add(neighbor);
                    //tileMap.SetCell(neighbor.x, neighbor.y, 5);
				}
			}
		}
        // Draw border
        foreach (Vector2Int r in regionTiles) { 
            foreach (Vector2Int neighbor in GetNeighbors(r.x, r.y)) {
                if (!regionTiles.Contains(neighbor)) {
                    tileMap.SetCell(r.x, r.y, 5);
		    	}
                if (neighbor - r == new Vector2Int(0, 1) && !visited.Contains(neighbor)) {
                    GD.Print("hey");
                    tileMap.SetCell(neighbor.x, neighbor.y, 7);
				}
		    }
		}
        //foreach (Vector2Int neighbor in neighbors) {
        //    if (!visited.Contains(neighbor)) {
        //        tileMap.SetCell(neighbor.x, neighbor.y, 2);
        //	}
        //}
        region.tiles = regionTiles;
        return region;
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
