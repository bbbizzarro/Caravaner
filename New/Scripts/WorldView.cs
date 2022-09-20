using Godot;
using System;
using Caravaner;

public class WorldView {
    PackedScene TileViewScene = (PackedScene)ResourceLoader.Load("res://New/TileView.tscn");
    Node2D _parent;
    int _pixelsPerUnit;
    int[] _textureFrames = new int[] {20, 55, 91};
    GameWorld _world;
    SpriteTable _spriteTable;

    public WorldView(Node2D parent, GameWorld world, int pixelsPerUnit) {
        _parent = parent;
        _pixelsPerUnit = pixelsPerUnit;
        _spriteTable = new SpriteTable();
        _world = world;
        world.TileCreatedEvent += InstantiateTile;
    }

    public void InstantiateTile(GameWorld.Tile tile) {
        Vector2Int pos = tile.Position;
        var tileView = (TileView)TileViewScene.Instance();
        tileView.Init(tile, _spriteTable);
        tileView.GlobalPosition = new Vector2(pos.x * _pixelsPerUnit, 
                                              pos.y * _pixelsPerUnit);
        _parent.AddChild(tileView);
    }
}
