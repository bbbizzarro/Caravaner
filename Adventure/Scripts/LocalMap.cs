using Godot;
using System;
using Caravaner;

public class LocalMap {
    bool[,] _map;
    public int Width {private set; get; }
    public int Height {private set; get; }
    public Vector2Int Bottom {private set; get;}
    public Vector2Int Top {private set; get;}
    RandomNumberGenerator _rng;
    int _pixelsPerUnit;

    public LocalMap(int width, int height, int pixelsPerUnit) {
        _rng = new RandomNumberGenerator();
        _rng.Randomize();
        _pixelsPerUnit = pixelsPerUnit;
        Width = width; Height = height;
        Bottom = ArrayToGrid(0, 0);
        Top = ArrayToGrid(Width, Height);
        _map = new bool[width, height];
        for (int x = 0; x < width; ++x) {
            for (int y= 0 ; y < height; ++y) {
                _map[x, y] = true;
            }
        }
    }

    public void Set(int x, int y, bool isOpen) {
        Vector2Int v = GridToArray(x, y);
        if (IsValid(v.x, v.y)) _map[v.x, v.y] = isOpen;
    }

    public bool IsOpen(int x, int y) {
        Vector2Int v = GridToArray(x, y);
        if (!IsValid(v.x, v.y)) return false;
        return _map[v.x, v.y];
    }

    private bool IsValid(int x, int y) {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }

    public Vector2Int GridToArray(int x, int y) {
        return new Vector2Int(x + Width / 2, y + Height / 2);
    }

    public Vector2Int ArrayToGrid(int x, int y) {
        return new Vector2Int(x - Width / 2, y - Height / 2);
    }
    public Vector2Int WorldToGrid(Vector2 v) {
        return new Vector2Int(Mathf.RoundToInt((float)v.x / (float)_pixelsPerUnit),
                              Mathf.RoundToInt((float)v.y / (float)_pixelsPerUnit));
    }
    public Vector2Int GridToWorld(Vector2 v) {
        return new Vector2Int(Mathf.RoundToInt((float)v.x * (float)_pixelsPerUnit),
                              Mathf.RoundToInt((float)v.y * (float)_pixelsPerUnit));
    }
}