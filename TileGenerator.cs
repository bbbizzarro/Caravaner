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
        gridMap.SetActiveRegions(r);
        //var tiles = FindOpenTiles(gridMap, r, 5);
        var tiles = FindOpenTiles(gridMap, r, 100);
        foreach (var t in tiles) {
            var rand = rng.Randf();
            if (rand > 0.9){
                //gridMap.Get(t.x, t.y).SetScene(false, "CampSite");
            }
            else if (rand > 0.3f) {
                //gridMap.Get(t.x, t.y).SetScene(false, "ResourcePoint");
                gridMap.Get(t.x, t.y).SetScene(false, "Plant");
            }
        }
    }

    public void BuildCity(GridMap gridMap, Region r) {
        RandList<string> imports = new RandList<string>() {"Food", "Scrap"};
        gridMap.SetActiveRegions(r);
        // Place city center;
        Vector2Int cityCenter = FindOpenTile(gridMap, r);
        if (cityCenter != Vector2Int.Zero) {
            gridMap.Get(cityCenter.x, cityCenter.y).SetScene(false, "CityCenter");
            r.Import = imports.Pop();
        }
        int citySize = 5;
        bool hasBank = rng.Randf() > 0.5f;

        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        Queue<Vector2Int> next = new Queue<Vector2Int>(gridMap.GetOpenNeighbors(cityCenter.x, cityCenter.y));
        while (next.Count > 0 && citySize > 0) {
            var n = next.Dequeue();
            if (!gridMap.Get(n.x, n.y).hasRoad && rng.Randf() > 0.1f) {
                float rand = rng.Randf();
                if (hasBank) {
                    gridMap.Get(n.x, n.y).SetScene(false, "Container");
                    hasBank = false;
                }
                else if (rand > 0.8f){
                    gridMap.Get(n.x, n.y).SetScene(false, "CityBlock");
                }
                else {
                    gridMap.Get(n.x, n.y).SetScene(false, "Exchange");
                }
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