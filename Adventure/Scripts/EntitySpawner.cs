using Godot;
using Caravaner;

public class EntitySpawner {
    Node _main;
    PackedScene PlayerPackedScene = (PackedScene)ResourceLoader.Load("res://Adventure/Scenes/Player.tscn");
    PackedScene StaticPackedScene = (PackedScene)ResourceLoader.Load("res://Adventure/Scenes/StaticEntity.tscn");
    EntityService _entityService;
    int _pixelsPerUnit;

    public EntitySpawner(Node main, int pixelsPerUnit, EntityService entityService) {
        _main = main;
        _entityService = entityService;
        _pixelsPerUnit = pixelsPerUnit;
    }

    public void SpawnStaticEntity(string name, Vector2Int gridPosition, LocalMap localMap) {
        var entity = (StaticEntity)StaticPackedScene.Instance();
        entity.Init(gridPosition, localMap, _pixelsPerUnit); 
        _main.AddChild(entity);
    }

    public PlayerEntity SpawnPlayer(Vector2 position) {
        var player = (PlayerEntity)PlayerPackedScene.Instance();
        _entityService.AddPlayer(player);
        player.Init(_pixelsPerUnit, 3f, _entityService);
        _main.AddChild(player);
        return player;
    }
}