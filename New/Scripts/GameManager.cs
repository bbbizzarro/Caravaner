using Godot;
using System;

public class GameManager : Node2D {
    [Export] int WorldHeight;
    [Export] int WorldWidth;
    [Export] int PixelsPerUnit;

    GameWorld _gameWorld;
    WorldView _worldView;
    Camera2D _mainCamera;
    PlayerModel _playerModel;
    UIView _uiView;

    public override void _Ready() {
        WorldGenerator worldGenerator = new WorldGenerator();
        _gameWorld = worldGenerator.Generate(WorldWidth, WorldHeight);
        _worldView = new WorldView(this, PixelsPerUnit);
        _worldView.RenderWorld(_gameWorld);
        _playerModel = new PlayerModel();
        ((PlayerView)GetNode("Player")).Init(_playerModel, PixelsPerUnit);
        _mainCamera = (Camera2D)GetNode("Camera2D");
        _mainCamera.GlobalPosition = new Vector2(WorldWidth / 2 * PixelsPerUnit, WorldHeight / 2 * PixelsPerUnit);
        _uiView = (UIView)GetNode("UI");
        _uiView.Init(_playerModel, _gameWorld);
    }
}
