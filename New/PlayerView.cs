using Godot;
using System;
using Caravaner;

public class PlayerView : KinematicBody2D {
    int _pixelsPerUnit;
    GameWorld.Entity _player;

    public void Init(GameWorld.Entity player, int PixelsPerUnit) {
        _player = player;
        _pixelsPerUnit = PixelsPerUnit;
    }

    public override void _Process(float delta) {
		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		MoveAndSlide(_pixelsPerUnit * _player.MovementSpeed * direction);
        UpdatePosition();
    }

    private void UpdatePosition() {
        var gridPosition = new Vector2Int(
            Mathf.RoundToInt(GlobalPosition.x / _pixelsPerUnit),
            Mathf.RoundToInt(GlobalPosition.y / _pixelsPerUnit)
        );
        _player.SetPosition(gridPosition.x, gridPosition.y);
    }
}