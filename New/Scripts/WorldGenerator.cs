using Godot;
using System.Collections.Generic;
using Caravaner;

public class WorldGenerator {
    RandomNumberGenerator _rng;
    public WorldGenerator() {
        _rng = new RandomNumberGenerator();
        _rng.Randomize();
    }

    public void Set(GameWorld gameWorld) {
        var terrainTypes = new List<string>() {"Dirt", "Grass"};
        for (int x = 0; x < gameWorld.Width; ++x) {
            for (int y = 0; y < gameWorld.Height; ++y) {
                if (_rng.Randf() > 0.5f) 
                    gameWorld.CreateTile( 
                        new GameWorld.Tile("Dirt", new Vector2Int(x, y), gameWorld));
                else
                    gameWorld.CreateTile(
                        new GrassTile("Grass", new Vector2Int(x, y), gameWorld));
            }
        }
    }
}