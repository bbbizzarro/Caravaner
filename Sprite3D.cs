using Godot;
using System;
using Caravaner;

[Tool]
public class Sprite3D : Node2D {
    /*
    (z)+ >(x)
       V(y)
    */
    
    [Export] protected Vector3 restPosition = new Vector3(0, 0, 1);
    [Export] protected float WorldScale = 64f;
    [Export] protected bool AlwaysBehind = false;
    [Export] protected float ZSortOffset = -0.05f;
    [Export] protected NodePath parentPath;

    protected Sprite3D parent;
    private float theta = 0;
    [Export] protected Vector3 offset = Vector3.Zero;
    protected Vector3 rOffset = Vector3.Zero;
    protected Vector3 currPosition;

    [Export] public float yOffset = 0f;

    public override void _Ready() {
        base._Ready();
        if (!parentPath.IsEmpty())
            parent = (Sprite3D)GetNode(parentPath);
        currPosition = restPosition;
    }

    public void SetOffset(Vector3 v) {
        offset = v;
    }

    private Vector2 Project2D(Vector3 v3) {
        return new Vector2(v3.x, v3.y);
    }

    private Vector3 RotateZ(Vector3 v, float t) {
        return new Vector3(v.x * Mathf.Cos(t) - v.y * Mathf.Sin(t),
                           v.x * Mathf.Sin(t) + v.y * Mathf.Cos(t),
                           v.z);
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
    
    public void SetRotation3D(float theta) {
        this.theta = theta;
    }

    public void RotateSprite(float delta) {
        theta += delta;
    }

    private Vector3 DoTransform(Vector3 v) {
        offset = new Vector3(offset.x, yOffset, offset.y);
        if (parent == null) 
            return RotateZ(RotateY(RotateX(v + offset, rOffset.x),  rOffset.y), rOffset.z);
        else 
            return RotateZ(RotateY(RotateX(v + offset + parent.currPosition, rOffset.x),  rOffset.y), rOffset.z);
    }

    private void TransformTo2D() {
        currPosition = DoTransform(restPosition);
        var rotPos =  RotateY(currPosition, theta);
        Position = WorldScale * ProjectIso(rotPos, -45f);
        if (!AlwaysBehind)
            ZIndex = (rotPos.z >= ZSortOffset) ? 0 : -1;
        else
            ZIndex = -2;
    }

    public override void _Process(float delta) {
        TransformTo2D();
    }
}
