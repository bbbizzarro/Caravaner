using Godot;
using System;

public class WorldView {
    PackedScene TileViewScene = (PackedScene)ResourceLoader.Load("res://New/TileView.tscn");
    Node2D _parent;
    int _pixelsPerUnit;
    int[] _textureFrames = new int[] {20, 55, 91};
    TileView[,] _spriteMap;
    GameWorld _world;
    SpriteTable _spriteTable;

    public WorldView(Node2D parent, int pixelsPerUnit) {
        _parent = parent;
        _pixelsPerUnit = pixelsPerUnit;
        _spriteTable = new SpriteTable();
    }

    public void RenderWorld(GameWorld world) {
        _world = world;
        _spriteMap = new TileView[world.Width, world.Height];
        for (int x = 0; x < world.Width; ++x) {
            for (int y = 0; y < world.Height; ++y) {
                var tileView = (TileView)TileViewScene.Instance();
                tileView.Init(_world.GetTileAt(x, y), _spriteTable);
                _spriteMap[x, y] = tileView;
                tileView.GlobalPosition = new Vector2(x * _pixelsPerUnit, y * _pixelsPerUnit);
                _parent.AddChild(tileView);
            }
        }
    }
}
