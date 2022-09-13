using Godot;
using System;
using Caravaner;

public class ProcAnim : Sprite {
    [Export] private float WorldScale = 64f;
    [Export] private float RotationSpeed = 5f;
    private Vector2 lastDir;
    [Export] private float targetRot;
    [Export] private float currRot;
    private float time = 0;
    private float weight = 1f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
    }

    public override void _Process(float delta) {
		var direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
        direction = direction.Normalized();
        if (direction.Length() < 0.1f) return;
        if (lastDir != direction) {
            lastDir = direction;
            targetRot = direction.Angle() - Mathf.Pi/ 2f;
            currRot = Rotation;
            //weight = 1f / (SmallAngleDiff(currRot, targetRot) + 0.01f);
            weight = 1f;
            time =0;
        }
        Rotation = Mathf.LerpAngle(currRot, targetRot, time);
        time = Mathf.Clamp(time + delta * weight * 10f, 0, 1);
    }

    public float SmallAngleDiff(float a, float b) {
        float mn = Utils.Mod(Mathf.Min(a, b), Utils.Tau);
        float mx = Utils.Mod(Mathf.Max(a, b), Utils.Tau);
        return Mathf.Min(mx - mn, mn + (Utils.Tau - mx));
    }
}
