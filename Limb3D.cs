using Godot;
using System.Collections.Generic;

public class Limb3D : Sprite3D{

    public override void _Process(float delta) {
        base._Process(delta);
        rOffset = new Vector3(Mathf.Sin(delta), 0, 0);
    }
}