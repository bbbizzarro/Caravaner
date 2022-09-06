using Godot;
using System;
using System.Collections.Generic;

public class Body3D : Node2D {
    [Export] private float Speed;
    List<Sprite3D> sprites = new List<Sprite3D>();

    public override void _Ready() {
        foreach (Node n in GetChildren()) {
            if (n.Name != "CameraTransform") {
                sprites.Add((Sprite3D)n);
            }
        }
    }

    public void RotateAll(float delta) {
        foreach (Sprite3D s in sprites) {
            s.RotateSprite(delta * Speed);
        }
    }

    public override void _Process(float delta) {
        if (Input.IsKeyPressed((int)KeyList.E)) {
            RotateAll(delta);
        }
        if (Input.IsKeyPressed((int)KeyList.Q)) {
            RotateAll(-delta);
        }
    }
}
