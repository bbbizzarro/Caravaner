using Godot;
using Caravaner;

public class GameController : Node2D {
    const int PixelsPerUnit = 12;
    EntitySpawner _entitySpawner;
    EntityService _entityService;
    PlayerEntity _player;
    LocalMapRenderer _renderer;
    LocalMap _localMap;
    RandomNumberGenerator _rng;
    UIController _ui;

    public override void _Ready() {
        _entityService = new EntityService();
        _entitySpawner = new EntitySpawner(this, PixelsPerUnit, _entityService);
        _player = _entitySpawner.SpawnPlayer(Vector2.Zero);
        _localMap = new LocalMap(42, 24, PixelsPerUnit);
        _renderer = ((LocalMapRenderer)GetNode("LocalMapRenderer")).Init(PixelsPerUnit, _player, _localMap);
        _rng = new RandomNumberGenerator();
        _rng.Randomize();
        //PlaceEntities();
        _renderer.UpdateOcclusionMap();
        InitializeWalls();
        InitializeEntities();
        _ui = (UIController)GetNode("UI");
        _player.InventoryUpdatedEvent += _ui.UpdatePlayerInventory;
    }

    public void InitializeWalls() {
        foreach (var n in GetNode("Walls").GetChildren()) {
            StaticEntity sn = ((StaticEntity)n);
            sn.Init(_localMap.WorldToGrid(sn.GlobalPosition), _localMap, PixelsPerUnit);
        }
    }

    public void InitializeEntities() {
        foreach (var n in GetNode("Entities").GetChildren()) {
            NPCEntity sn = ((NPCEntity)n);
            sn.Init(PixelsPerUnit, 2f, _entityService);
        }
    }

    public void PlaceEntities() {
        for (int x = _localMap.Bottom.x; x < _localMap.Top.x; ++x) {
            for (int y = _localMap.Bottom.y; y < _localMap.Top.y; ++y) {
                if (_rng.Randf() > 0.9f) {
                    _entitySpawner.SpawnStaticEntity("test", new Vector2Int(x, y), _localMap);
                }
            }
        }
    }
}