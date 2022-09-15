using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public delegate void HandleTileChange(int x, int y);

public class GameWorld : ITerrainMap {

    public int Width {private set; get;}
    public int Height {private set; get;}
    public event HandleTileChange TileChangedEvent;
    Tile[,] _world;

    public GameWorld(int width, int height) {
        Width = width; Height = height;
        _world = new Tile[width, height];
        for (int x = 0; x < width; ++x) {
            for (int y = 0; y < height; ++y) {
                _world[x, y] = new Tile(TerrainType.Dirt);
            }
        }
    }

    public void SetTerrainAt(int x, int y, TerrainType terrain) {
        if (IsInBounds(x, y)) {
            _world[x, y].Terrain = terrain;
            TileChangedEvent?.Invoke(x, y);
        }
    }

    public TerrainType GetTerrainAt(int x, int y) {
        if (IsInBounds(x, y))
            return _world[x, y].Terrain;
        else return TerrainType.Null;
    }

    private bool IsInBounds(int x, int y) {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }

    public class Tile {
        public TerrainType Terrain;
        public Tile(TerrainType terrain) {
            Terrain = terrain;
        }
    }
}

public enum TerrainType {
    Null,
    Dirt,
    Grass
}

public interface ITerrainMap {
    void SetTerrainAt(int x, int y, TerrainType terrain);
    TerrainType GetTerrainAt(int x, int y);
}

public class WorldGenerator {
    RandomNumberGenerator _rng;
    public WorldGenerator() {
        _rng = new RandomNumberGenerator();
        _rng.Randomize();
    }

    public GameWorld Generate(int width, int height) {
        GameWorld gameWorld = new GameWorld(width, height);
        var terrainTypes = (TerrainType[])Enum.GetValues(typeof(TerrainType));
        for (int x = 0; x < width; ++x) {
            for (int y = 0; y < height; ++y) {
                gameWorld.SetTerrainAt(x, y, terrainTypes[_rng.RandiRange(1, terrainTypes.Length - 1)]);
            }
        }
        return gameWorld;
    }
}