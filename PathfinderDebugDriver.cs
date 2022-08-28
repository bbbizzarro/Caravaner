using Caravaner;
using Godot;
using System;
using System.Collections.Generic;

public class PathfinderDebugDriver : Node2D {
    private PathFinder pf;
    private DebugMap map;
    private HashSet<Vector2Int> nexts = new HashSet<Vector2Int>();
    private Vector2Int start;
    private Vector2Int end;
    private float mag;
    private PackedScene textLabelScene = (PackedScene)ResourceLoader.Load("res://FloatingLabel.tscn");
    private Dictionary<Vector2Int, FloatingLabel> labels = new Dictionary<Vector2Int, FloatingLabel>();
    private int width;
    private int height;

    public override void _Ready() {
        width = 10;
        height = 10;
        map = new DebugMap(width, height);
        pf = new PathFinder(100, map);
        start = new Vector2Int(0, 0);
        end = new Vector2Int(9, 9);
        pf.StartFindPath(start, end);
        mag = (end - start).Magnitude();
        GD.Print(String.Format("Total distance = {0}", mag));
        SetLabels();
    }

    public void SetLabels() {
        for (int x = 0; x < width; ++x) {
            for (int y = 0 ; y < height; ++y) {
                FloatingLabel l = (FloatingLabel)textLabelScene.Instance();
                l.GlobalPosition = new Vector2((x + 0.5f)*64, (y + 0.5f)*64);
                labels.Add(new Vector2Int(x, y), l);
                AddChild(l);
                l.SetLabel((-1).ToString());
            }
        }
    }

    public override void _Process(float delta) {
        if (Input.IsActionJustPressed("interact")) {
            var v = pf.FindNext();
            if (!nexts.Contains(v)) {
                nexts.Add(v);
            }
            Update();
        }

        if (Input.IsActionJustPressed("ui_click")) {
            Vector2 m = GetGlobalMousePosition();
            int x = Mathf.RoundToInt(m.x /64f );
            int y = Mathf.RoundToInt(m.y /64f );
            map.Set(x, y, !map.Get(x, y));
            Update();
        }
    }

    public override void _Draw() {
        foreach (var v in nexts) {
            float d = pf.GetDistance(v);
            Color c = new Color(1, 1, 1);
            if (d >= 0) {
                float m = d / mag; 
                c = new Color(m, m, m);
            }
            labels[v].SetLabel((d + pf.GetHeuristic(v)).ToString());
            DrawRect(new Rect2(new Vector2(v.x * 64, v.y *64), new Vector2(64, 64)), 
                c);
        }
        for (int x = 0; x< width; ++x) {
            for (int y = 0; y < height; ++y) {
                if (!map.Get(x, y)) {
                    DrawRect(new Rect2(new Vector2(x * 64, y *64), new Vector2(64, 64)), 
                        new Color(1, 0, 0));
                }
            }
        }

    }
}

public class DebugMap : IPathGrid {

    public int Width {private set; get;}
    public int Height {private set; get;}
    bool[,] map;
    private readonly HashSet<Region> activeRegions = new HashSet<Region>();
	Vector2Int[] dirs = {new Vector2Int(-1, 0),  new Vector2Int(0, -1),
                         new Vector2Int( 1, 0),  new Vector2Int(0,  1)};


    public DebugMap(int width, int height) {
        Width = width;
        Height = height;
        map = new bool[width,height];
        for (int x = 0; x< width; ++x) {
            for (int y = 0; y < height; ++y) {
                map[x,y] = true;
            }
        }
    }

    public bool Get(int x, int y) {
        if (x >= 0 && y >= 0 && x < Width && y < Height) {
            return map[x,y];
        }
        else {
            return false;
        }
    }

    public void Set(int x, int y, bool b) {
        if (x >= 0 && y >= 0 && x < Width && y < Height) {
            map[x,y] = b;
        }
    }

    public List<Vector2Int> GetOpenNeighbors(int x, int y) {
        var center = new Vector2Int(x, y);
        var neighbors = new List<Vector2Int>();
        foreach (var dir in dirs) {
            var neighbor = center + dir;
            if (IsOpen(neighbor.x, neighbor.y)) {
                neighbors.Add(neighbor);
            }
        }
        return neighbors;
    }

    public bool IsOpen(int x, int y) {
        return x >= 0 && y >= 0 && x < Width && y < Height && map[x,y];
    }
}