using System.Collections.Generic;
using Godot;

public class WorldSim {

    public List<Option> GetOptions(GameWorld world, PlayerModel playerModel) {
        List<Option> options = new List<Option>();
        if (world.GetTerrainAt(playerModel.Position.x, playerModel.Position.y) == 
            TerrainType.Grass) {
            options.Add(new ChangeTileOption("Pluck some grass", 
                playerModel.Position.x, playerModel.Position.y, 
                world, TerrainType.Dirt));
        }
        return options;
    }
}

public class Option {
    public string Name {private set; get; }

    public Option(string name) {
        Name = name;
    }

    public override string ToString() {
        return Name;
    }

    public virtual void Execute() {
    }
}

public class ChangeTileOption : Option {
    int _x;
    int _y;
    TerrainType _result;
    GameWorld _gameWorld;

    public ChangeTileOption(string name, int x, int y, GameWorld gameWorld, TerrainType result) : base(name) {
        _x = x; _y = y; _result = result; _gameWorld = gameWorld;
    }

    public override void Execute() {
        _gameWorld.SetTerrainAt(_x, _y, _result);
    }
}