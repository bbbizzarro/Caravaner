using Godot;
using System;

public delegate void InteractHandler();
public class Interactor : Area2D {
    InteractionTarget _active;
    public event InteractHandler InteractEvent;
    PlayerEntity _playerEntity;

    public Interactor Init(PlayerEntity playerEntity) {
        Connect("area_entered", this, nameof(HandleAreaEntered));
        Connect("area_exited", this, nameof(HandleAreaExited));
        _playerEntity = playerEntity;
        return this;
    }

    public void HandleAreaEntered(Area2D area) {
        _active = (InteractionTarget)area;
    }
    public void HandleAreaExited(Area2D area) {
        var interactor = (InteractionTarget)area;
        if (_active == area) {
            _active = null;
        }
    }

    public void Interact() {
        if (_active != null) {
            _active.Interact(_playerEntity);
        }
    }
}
