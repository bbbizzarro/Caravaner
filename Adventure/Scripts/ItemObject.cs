using Godot;
using System;

public class ItemObject : KinematicBody2D {
    InteractionTarget _interactionTarget;
    float _magnitude;
    Vector2 _direction;
    const float Friction = 0.5f;
    Timer _timer;
    float _flyTime = 0.2f;
    PackedScene _damageCastScene = (PackedScene)ResourceLoader.Load("res://Adventure/Scenes/DamageCast.tscn");
    DamageCast _damageCast;

    public override void _Ready() {
        _interactionTarget = ((InteractionTarget)GetNode("InteractionTarget"));
        _interactionTarget.InteractionEvent += PickUp;
        _timer = (Timer)GetNode("Timer");
        _timer.Connect("timeout", this, nameof(Stop));
        _timer.Start(_flyTime);
        AddChild(_damageCast);
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
        return this;
    }

    public void PickUp(PlayerEntity playerEntity) {
        playerEntity.Carrier.Set();
        QueueFree();
    }
}
