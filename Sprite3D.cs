using Godot;
using System;

public class Sprite3D : Node2D {
    [Export] protected Vector3 headPosition = new Vector3(0, 0, 1);
    [Export] protected float WorldScale = 64f;
    [Export] protected bool AlwaysBehind = false;
    private float theta = 0;

    private Vector2 Project2D(Vector3 v3) {
        return new Vector2(v3.x, v3.y);
    }

    private Vector3 RotateX(Vector3 v, float t) {
        return new Vector3(v.x,
                           v.y * Mathf.Cos(t) - v.z * Mathf.Sin(t),
                           v.y * Mathf.Sin(t) + v.z * Mathf.Cos(t));
    }

    private Vector3 RotateY(Vector3 v, float t) {
        return new Vector3(v.x * Mathf.Cos(t) + v.z * Mathf.Sin(t),
                           v.y,
                           -v.x * Mathf.Sin(t) + v.z * Mathf.Cos(t));
    }

    private Vector2 ProjectIso(Vector3 v3, float d) {
        return Project2D(RotateX(v3, d));
    }

    public void RotateSprite(float delta) {
        theta += delta;
    }

    public void TransformTo2D() {
        var rotPos =  RotateY(headPosition, theta);
        Position = WorldScale * ProjectIso(rotPos, -45f);
        if (!AlwaysBehind)
            ZIndex = (rotPos.z >= 0) ? 0 : -1;
    }

    public override void _Process(float delta) {
        TransformTo2D();
    }
}
