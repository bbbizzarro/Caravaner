using Godot;
using System.Collections.Generic;
using Caravaner;

public class SpringSprite : Sprite3D {
    private float t = 0;
    [Export] private float period = 20f;
    [Export] private float amplitude = 0.05f;

    public override void _Ready() {
        base._Ready();
    }

    public override void _Process(float delta) {
        t = Utils.Mod(t + delta, Utils.Tau);
        //offset = new Vector3(0.5f* amplitude * Mathf.Sin(0.5f * t * period), amplitude * Mathf.Sin(t * period), 0);
        offset = new Vector3(0, amplitude * Mathf.Sin(period * t), 0);
        base._Process(delta);
    }
}