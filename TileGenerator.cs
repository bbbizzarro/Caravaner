using Godot;
using System.Collections.Generic;
using Caravaner;
using System.Linq;

public class TileGenerator {
    RandomNumberGenerator rng = new RandomNumberGenerator();

    public TileGenerator() {
        rng.Randomize();
    }

    public void Generate(GridMap gridMap) {
        gridMap.ActivateAllRegions();
        SetRegionTypes(gridMap);
        foreach (var r in gridMap.GetRegions()) {
            if (r.type == 2) {
                BuildCity(gridMap, r);
            }
            else if (r.type == 1) {
                PlaceResources(gridMap, r);
            }
        }
    }

    public void PlaceResources(GridMap gridMap, Region r) {
        var tiles = FindOpenTiles(gridMap, r, 5);
        foreach (var t in tiles) {
            if (rng.Randf() > 0.5f){
                gridMap.Get(t.x, t.y).SetScene(true, "ResourcePoint");
            }
        }
    }

    public void BuildCity(GridMap gridMap, Region r) {
        // Place city center;
        Vector2Int cityCenter = FindOpenTile(gridMap, r);
        if (cityCenter != Vector2Int.Zero) {
            gridMap.Get(cityCenter.x, cityCenter.y).SetScene(false, "CityCenter");
        }
        int citySize = 5;

        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        Queue<Vector2Int> next = new Queue<Vector2Int>(gridMap.GetOpenNeighbors(cityCenter.x, cityCenter.y));
        while (next.Count > 0 && citySize > 0) {
            var n = next.Dequeue();
            if (!gridMap.Get(n.x, n.y).hasRoad && rng.Randf() > 0.1f) {
                gridMap.Get(n.x, n.y).SetScene(false, "CityBlock");
                citySize -= 1;
            }
            visited.Add(n);
            foreach (var t in gridMap.GetOpenNeighbors(n.x, n.y)) {
                if (!visited.Contains(t)) next.Enqueue(t);
            }
        }
    }

    private List<Vector2Int> FindOpenTiles(GridMap gridMap, Region r, int count) {
        RandList<Vector2Int> tiles = 
            new RandList<Vector2Int>(r.tiles.
                Where(t => gridMap.IsOpen(t.x, t.y) && !gridMap.Get(t.x, t.y).hasRoad));
        List<Vector2Int> result = new List<Vector2Int>();
        for (int i = 0; i < count; ++i) {
            if (tiles.IsEmpty()) return result;
            result.Add(tiles.Pop());
        }
        return result;
    }

    private Vector2Int FindOpenTile(GridMap gridMap, Region r) {
        RandList<Vector2Int> tiles = 
            new RandList<Vector2Int>(r.tiles.
                Where(t => gridMap.IsOpen(t.x, t.y) && !gridMap.Get(t.x, t.y).hasRoad));
        if (tiles.Count != 0){
            return tiles.Pop();
        } 
        else return Vector2Int.Zero;
    }


    public void SetRegionTypes(GridMap gridMap) {
        RandList<bool> cityBools = new RandList<bool>();
        int cityFraction = gridMap.NumOpenRegions/ 4;
        //cityFraction =  gridMap.NumOpenRegions;
        for (int i = 0; i < cityFraction; ++i) {
            cityBools.Add(true);
        }
        for (int i = cityFraction; i < gridMap.NumOpenRegions; ++i) {
            cityBools.Add(false);
        }
        GD.Print(gridMap.NumOpenRegions, " ", gridMap.NumOpenRegions / 4);
        foreach (var r in gridMap.GetOpenRegions()) {
            if (cityBools.Pop()) {
                r.type = 2;
            }
            else {
                r.type = 1;
            }
        }
    }
}