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

    public void SpawnPlayer(Vector2 position) {
        var player = (PlayerEntity)PlayerPackedScene.Instance();
        _main.AddChild(player);
        player.Init(_pixelsPerUnit, 4f, _entityService);
    }
}