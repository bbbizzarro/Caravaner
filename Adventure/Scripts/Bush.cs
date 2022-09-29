using Godot;
using System;

public class Bush : StaticEntity {
    bool _isActive = true;
    Interactor _interactor;
    PackedScene PickUpScene = (PackedScene)ResourceLoader.Load("res://Adventure/Scenes/PickUp.tscn");

    public override void _Ready() {
        base._Ready();
        _interactor = (Interactor)GetNode("Interactor");
        _interactor.ReceiveEvent += HandleInteraction;
    }

    public void HandleInteraction(Node node) {
        if (_isActive) {
            GD.Print("hallo!");
            var scene = (PickUp)PickUpScene.Instance();
            GetParent().AddChild(scene);
            scene.GlobalPosition = GlobalPosition;
        }
    }
}
