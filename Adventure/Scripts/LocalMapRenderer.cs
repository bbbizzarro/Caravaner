using Godot;
using System;
using Caravaner;

public class LocalMapRenderer : Node2D {
    TileMap _fov;
    LocalMap _localMap;
    int _pixelsPerUnit = 12;
    PlayerEntity _playerEntity;
    Vector2Int _lastTarget;
    Vector2Int _lastPlayerPosition;

    public override void _Ready() {
        _fov = (TileMap)GetNode("FOV");
        _localMap = new LocalMap(100, 100);
        _localMap.Set(-1, 1, false);
        DrawLine(new Vector2Int(0, 0), new Vector2Int(5, 3));
    }

    public override void _Process(float delta) {
        Vector2Int v = WorldToGrid(GetGlobalMousePosition());
        Vector2Int pv =WorldToGrid(_playerEntity.GlobalPosition) ;
        if (_lastTarget != v || _lastPlayerPosition != pv) {
            _lastTarget = v;
            _lastPlayerPosition = pv;
            _fov.Clear();
            //DrawLine(Vector2Int.Zero, WorldToGrid(GetGlobalMousePosition()));
            DrawLine(WorldToGrid(_playerEntity.GlobalPosition),
                     WorldToGrid(GetGlobalMousePosition()));
        }
        if (Input.IsActionJustPressed("ui_click")) {
        }
    }

    private Vector2Int WorldToGrid(Vector2 v) {
        return new Vector2Int(Mathf.RoundToInt((float)v.x / (float)_pixelsPerUnit),
                              Mathf.RoundToInt((float)v.y / (float)_pixelsPerUnit));
    }

    public LocalMapRenderer Init(int pixelsPerUnit, PlayerEntity playerEntity) {
        _pixelsPerUnit = pixelsPerUnit;
        _playerEntity = playerEntity;
        return this;
    }
    // Maybe we can linearly interpolate what's visible based on 
    // how close the player is to surrounding tiles.

    // (0, 0) -> (1, -5)

    public void DrawLine(Vector2Int from, Vector2Int to) {
        // translate from to zero;
        Vector2Int translate = from;
        from -= translate;
        to -= translate;
        int dx = to.x - from.x;
        int dy = to.y - from.y;
        if (Mathf.Abs(dx) > Mathf.Abs(dy)) {
            float m = (float)dy / (float)dx;
            int sign = Mathf.Sign(dx);
            if (sign == 0) return;
            for (int x = from.x; x != to.x + sign; x += sign) {
                Vector2Int v = new Vector2Int(x + translate.x, Mathf.RoundToInt(m * (float)x) + translate.y);
                if (!_localMap.IsOpen(v.x, v.y)) return;
                _fov.SetCell(v.x, v.y, 0); 
            }
        }
        else {
            float m = (float)dx / (float)dy;
            int sign = Mathf.Sign(dy);
            if (sign == 0) return;
            for (int y = from.y; y != to.y + sign; y += sign) {
                Vector2Int v = new Vector2Int(Mathf.RoundToInt(m * (float)y) + translate.x, y + translate.y);
                if (!_localMap.IsOpen(v.x, v.y)) return;
                _fov.SetCell(v.x, v.y, 0);
            }
        }
    }

    public void RenderFOV() {
        for (int x = 0; x < _localMap.Width; ++x) {
            for (int y = 0; y < _localMap.Height; ++y) {
                if (_localMap.IsOpen(x, y)) {
                    _fov.SetCell(x, y, -1);
                }
                else {
                    _fov.SetCell(x, y, 0);
                }
            }
        }
    }
}
