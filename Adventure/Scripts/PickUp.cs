using Godot;
using System;

public class PickUp : KinematicBody2D {


    [Export] float speed = 10;
    string ItemID;
    Area2D sensor;
    Sensor target;

    public override void _Ready() {
        sensor = (Area2D)GetNode("Sensor");
        sensor.Connect("area_entered", this, nameof(OnSensorDetect));
    }

    public override void _Process(float delta) {
        if (target != null) {
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
