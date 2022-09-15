using Godot;
using System;

public class WorldView {
    PackedScene TileViewScene = (PackedScene)ResourceLoader.Load("res://New/TileView.tscn");
    Node2D _parent;
    int _pixelsPerUnit;
    int[] _textureFrames = new int[] {20, 55, 91};
    Sprite[,] _spriteMap;
    GameWorld _world;

    public WorldView(Node2D parent, int pixelsPerUnit) {
        _parent = parent;
        _pixelsPerUnit = pixelsPerUnit;
    }

    public void RenderWorld(GameWorld world) {
        _world = world;
        world.TileChangedEvent += SetTileView;
        _spriteMap = new Sprite[world.Width, world.Height];
        for (int x = 0; x < world.Width; ++x) {
            for (int y = 0; y < world.Height; ++y) {
                var tileView = (Sprite)TileViewScene.Instance();
                tileView.Frame = _textureFrames[(int)_world.GetTerrainAt(x, y)];
                _spriteMap[x, y] = tileView;
                tileView.GlobalPosition = new Vector2(x * _pixelsPerUnit, y * _pixelsPerUnit);
                _parent.AddChild(tileView);
            }
        }
    }

    public void SetTileView(int x, int y) {
        _spriteMap[x,y].Frame = _textureFrames[(int)_world.GetTerrainAt(x, y)];
    }
}
