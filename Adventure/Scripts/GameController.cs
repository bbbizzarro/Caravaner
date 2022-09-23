using Godot;

public class GameController : Node2D {
    const int PixelsPerUnit = 12;
    EntitySpawner _entitySpawner;
    EntityService _entityService;
    PlayerEntity _player;
    LocalMapRenderer _renderer;

    public override void _Ready() {
        _entityService = new EntityService();
        _entitySpawner = new EntitySpawner(this, PixelsPerUnit, _entityService);
        _player = _entitySpawner.SpawnPlayer(Vector2.Zero);
        _renderer = ((LocalMapRenderer)GetNode("LocalMapRenderer")).Init(PixelsPerUnit, _player);
    }
}