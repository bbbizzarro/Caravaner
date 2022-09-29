using Godot;
using System;

public class PickUp : KinematicBody2D {


    [Export] float speed = 10;
    string ItemID = "DebugItem";
    Area2D sensor;
    Sensor target;
    Timer _timer;
    bool _isReady;

    public override void _Ready() {
        sensor = (Area2D)GetNode("Sensor");
        sensor.Connect("area_entered", this, nameof(OnSensorDetect));
        _timer = (Timer)GetNode("Timer");
        _timer.Connect("timeout", this, nameof(SetReady));
        _timer.Start(0.5f);
    }

    private void SetReady() {
        _isReady = true;
    }

    public override void _Process(float delta) {
        if (target != null && _isReady) {
            Move(target);
        }
    }

    private void Move(Sensor target) {
        Vector2 difference = (target.GlobalPosition - GlobalPosition);
        if (difference.Length() < Globals.Epsilon * Globals.PixelsPerUnit) {
            target.AddItem(this, ItemID);
            QueueFree();
        }
        MoveAndSlide(speed * Globals.PixelsPerUnit * difference.Normalized());
    }

    public void OnSensorDetect(Area2D detectable) {
        if (target != null) return;
        if (detectable.IsInGroup("Player")) {
            target = (Sensor)detectable;
        }
    }
}
