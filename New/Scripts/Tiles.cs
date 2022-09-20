using Caravaner;
using Godot;
using System.Collections.Generic;

public class GrassTile : GameWorld.Tile {
    public GrassTile(string name, Vector2Int position, GameWorld gameWorld) : base(name, position, gameWorld) {}

    public override List<string> Preview(GameWorld.Entity entity) {
        var options = new List<string>() {"Pluck", "Dig"};
        if (entity.Items.HasItem("Gnomish Hat", 1)) {
            options.Add("Call");
        }
        return options;
    }

    public override void Interact(GameWorld.Entity entity, int option) {
        var options = Preview(entity);
        option = Mathf.Clamp(option, 0, options.Count - 1);
        if (options[option] == "Call") {
            _gameWorld.CreateTile( 
                new GameWorld.Tile("Dirt", Position, _gameWorld));
            entity.Items.TryToRemove("Gnomish Hat", 1);
            entity.Items.TryToAdd(new Status("Friendly Gnome"), 1);
        }
        else {
            _gameWorld.CreateTile( 
                new GameWorld.Tile("Dirt", Position, _gameWorld));
            entity.Items.TryToAdd(new Status("Gnomish Hat"), 1);
        }
    }
}