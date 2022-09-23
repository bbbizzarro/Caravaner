using Godot;

public class EntitySpawner {
    Node _main;
    PackedScene PlayerPackedScene = (PackedScene)ResourceLoader.Load("res://Adventure/Scenes/Player.tscn");
    EntityService _entityService;
    int _pixelsPerUnit;

    public EntitySpawner(Node main, int pixelsPerUnit, EntityService entityService) {
        _main = main;
        _entityService = entityService;
        _pixelsPerUnit = pixelsPerUnit;
        GD.Print(_pixelsPerUnit);
    }

    public PlayerEntity SpawnPlayer(Vector2 position) {
        var player = (PlayerEntity)PlayerPackedScene.Instance();
        player.Init(_pixelsPerUnit, 4f, _entityService);
        _main.AddChild(player);
        return player;
    }
}