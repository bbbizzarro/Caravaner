using Godot;
using System;
using System.Collections.Generic;
using Caravaner;

public class Body3D : Node2D {
    [Export] private float Speed;
    [Export] private float WorldScale = 64f;
    List<Sprite3D> sprites = new List<Sprite3D>();

    public override void _Ready() {
        foreach (Node n in GetNode("Parts").GetChildren()) {
            sprites.Add((Sprite3D)n);
        }
    }

    public void RotateAll(float delta) {
        foreach (Sprite3D s in sprites) {
            s.RotateSprite(delta * Speed);
        }
    }

    public void SetRotateAll(float radians) {
        foreach (Sprite3D s in sprites) {
            s.SetRotation3D(radians);
        }

    }
    private Vector2 lastDir;
    private float targetRot;
    private float currRot;
    private float time = 0;
    private float weight = 1f;
    private float rot3D;

    public override void _Process(float delta) {
        if (Input.IsKeyPressed((int)KeyList.E)) {
            RotateAll(delta);
        }
        if (Input.IsKeyPressed((int)KeyList.Q)) {
            RotateAll(-delta);
        }


		var direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
        direction = new Vector2(-direction.x, direction.y);
        direction = direction.Normalized();

        if (direction.Length() < 0.1f) return;
        if (lastDir != direction) {
            lastDir = direction;
            targetRot = direction.Angle() - Mathf.Pi/ 2f;
            currRot = rot3D;
            //weight = targetRot - currRot;
            //weight = Mathf.Abs(Utils.Mod((weight + Mathf.Pi / 2f), 2f * Mathf.Pi) - Mathf.Pi / 2f);
            //if (weight == 0 ) weight = 1f;
            //else weight = 1f / weight;
            //weight = targetRot - currRot;
            //weight = Mathf.Abs(Utils.Mod((weight + Mathf.Pi / 2f), 2f * Mathf.Pi) - Mathf.Pi / 2f) / (Mathf.Pi/2f);
            time =0;
        }
        rot3D = Mathf.LerpAngle(currRot, targetRot, time);
        SetRotateAll(rot3D);
        time = Mathf.Clamp(time + delta * weight * 10f, 0, 1);

    }
}
