using Godot;
using System;

public class Tree : StaticEntity{
    PackedScene PickUpScene = (PackedScene)ResourceLoader.Load("res://Adventure/Scenes/PickUp.tscn");

    public override void Destroy() {
        var scene = (PickUp)PickUpScene.Instance();
        GetParent().AddChild(scene);
        scene.GlobalPosition = GlobalPosition;
        base.Destroy();
    }
}
