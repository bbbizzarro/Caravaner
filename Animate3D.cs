using Godot;
using System;
using System.Collections.Generic;

public class Animate3D : Sprite3D {
    /*
    (z)+ >(x)
       V(y)
    */
    private Animation3D bobAnimation;

    public override void _Ready() {
        Vector3 start = restPosition;
        Vector3 mid = restPosition + new Vector3(0, 0.5f, 0f);
        Vector3 end = start;
        bobAnimation = new Animation3D(new List<Vector3>() {start, mid, end}, 1f);
    }

    public override void _Process(float delta) {
        //headPosition = bobAnimation.Animate(delta);
        //TransformTo2D();
    }
}
