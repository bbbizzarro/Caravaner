using Godot;
using System.Collections.Generic;


public class Animation3D {
    private List<Vector3> keyFrames;
    [Export] private float time = 0;
    private float speed;
    private int currIndex = 0; 
    private int nextIndex = 0;

    public Animation3D(IEnumerable<Vector3> keyFrames, float speed) {
        this.keyFrames = new List<Vector3>(keyFrames);
        if (this.keyFrames.Count == 0) {
            GD.PrintErr("Empty keyframes.");
        }
        else {
            this.speed = this.keyFrames.Count * speed;
        }
    }

    public void Start() {
        time = 0;
    }

    public void Stop() {
        time = 0;
    }

    public Vector3 Animate(float seconds) {
        time = Advance(seconds);
        Vector3 currKeyFrame = keyFrames[currIndex];
        Vector3 nextKeyFrame = keyFrames[nextIndex];
        return currKeyFrame * (1f - time) + nextKeyFrame * time;
    }


    private float Advance(float delta) {
        float newTime = Mathf.Max(0, time + delta * speed);
        if (newTime > 1) {
            newTime %= 1;
            currIndex = (currIndex + 1) % keyFrames.Count;
            nextIndex = (currIndex + 1) % keyFrames.Count;
        }
        return newTime;
    }
}