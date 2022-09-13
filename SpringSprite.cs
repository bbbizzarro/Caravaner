using Godot;
using System.Collections.Generic;
using Caravaner;

public class SpringSprite : Sprite3D {
    private float t = 0;
    [Export] private float period = 20f;
    [Export] private float amplitude = 0.05f;

    public override void _Process(float delta) {
        t = Utils.Mod(t + delta, Utils.Tau);
        offset = new Vector3(0, amplitude * Mathf.Sin(t * period), 0);
        base._Process(delta);
    }
}