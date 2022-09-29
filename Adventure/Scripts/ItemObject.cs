using Godot;
using System;

public class ItemObject : KinematicBody2D {
    Interactor _interactor;
    float _magnitude;
    Vector2 _direction;
    const float Friction = 0.5f;
    Timer _timer;
    float _flyTime = 0.2f;
    PackedScene _damageCastScene = (PackedScene)ResourceLoader.Load("res://Adventure/Scenes/DamageCast.tscn");
    DamageCast _damageCast;

    public override void _Ready() {
        _interactor = (Interactor)GetNode("Interactor");
        _interactor.ReceiveEvent += PickUp;
        _timer = (Timer)GetNode("Timer");
        _timer.Connect("timeout", this, nameof(Stop));
        _timer.Start(_flyTime);
    }

    public override void _Process(float delta) {
        MoveAndSlide(_magnitude * _direction);
    }

    public void Stop() {
        _magnitude = 0;
    }

    public ItemObject Init(Vector2 direction, float magnitude, float flyTime, Area2D origin) {
        _direction = direction;
        _magnitude = magnitude;
        _flyTime = flyTime;
        _damageCast = ((DamageCast)_damageCastScene.Instance()).Init(10, flyTime, origin);
        _damageCast.SetOneHit();
        AddChild(_damageCast);
        return this;
    }

    public void PickUp(Node node) {
        //playerEntity.Carrier.Set();
        QueueFree();
    }
}
