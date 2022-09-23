using Godot;
using System;
using System.Collections.Generic;

public class BresenhamDebug : Node2D {

    public override void _Ready() {
        base._Ready();
    }


    public override void _Draw() {
        Bresenham b = new Bresenham();
        var l = b.DrawLine(0, 5, 0, 30);
        foreach (var v in l) {
            DrawRect(new Rect2(12 * v, new Vector2(12, 12)), new Color(1, 1, 1, 1));
        }
    }
}

public class Bresenham {
    public List<Vector2> DrawLine(int x0, int x1, int y0, int y1) {
        var points = new List<Vector2>();
        int dx = x1 - x0;
        int dy = y1 - y0;
        int D = 2*dy - dx;
        int y = y0;
        for (int x = x0; x <= x1; ++x) {
            points.Add(new Vector2(x, y));
            if (D > 0) {
                y = y + 1;
                D = D - 2*dx;
            }
            D = D + 2*dy;
        }
        return points;
    }
}
