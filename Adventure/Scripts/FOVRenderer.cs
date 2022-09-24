using Godot;
using Caravaner;

public class FOVRenderer {
    TileMap _fov;
    LocalMap _localMap;
    int _pixelsPerUnit = 12;
    PlayerEntity _playerEntity;
    Vector2Int _lastTarget;
    Vector2Int _lastPlayerPosition;

    public FOVRenderer(LocalMap localMap, TileMap fovTileMap, int pixelsPerUnit, PlayerEntity playerEntity) {
        _localMap = localMap;
        _fov = fovTileMap;
        _pixelsPerUnit = pixelsPerUnit;
        _playerEntity = playerEntity;
        Clear();
    }

    public void Render() {
        Vector2Int pv = WorldToGrid(_playerEntity.GlobalPosition) ;
        if (_lastPlayerPosition != pv) {
            _lastPlayerPosition = pv;
            RayCastAll();
        }
    }

    private Vector2Int WorldToGrid(Vector2 v) {
        return new Vector2Int(Mathf.RoundToInt((float)v.x / (float)_pixelsPerUnit),
                              Mathf.RoundToInt((float)v.y / (float)_pixelsPerUnit));
    }

    public void RayCastAll() {
        Vector2Int playerPosition = WorldToGrid(_playerEntity.GlobalPosition);
        // Top
        for (int x = _localMap.Bottom.x; x < _localMap.Top.x; ++x) {
            DrawLine(playerPosition,
                     new Vector2Int(x, _localMap.Top.y - 1));
        }
        for (int x = _localMap.Bottom.x; x < _localMap.Top.x; ++x) {
            DrawLine(playerPosition,
                     new Vector2Int(x, _localMap.Bottom.y));
        }
        for (int y = _localMap.Bottom.y; y < _localMap.Top.y; ++y) {
            DrawLine(playerPosition,
                     new Vector2Int(_localMap.Top.x - 1, y));
        }
        for (int y = _localMap.Bottom.y; y < _localMap.Top.y; ++y) {
            DrawLine(playerPosition,
                     new Vector2Int(_localMap.Bottom.x, y));
        }
    }

    // Maybe we can linearly interpolate what's visible based on 
    // how close the player is to surrounding tiles.

    // (0, 0) -> (1, -5)

    private void Clear() {
        for (int x = 0; x < _localMap.Width; ++x) {
            for (int y = 0; y < _localMap.Height; ++y) {
                Vector2Int pos = _localMap.ArrayToGrid(x, y);
                _fov.SetCell(pos.x, pos.y, 0);//, autotileCoord:new Vector2(0, 0));
            }
        }
    }

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
                _fov.SetCell(v.x, v.y, -1); 
                if (!_localMap.IsOpen(v.x, v.y)) return;
            }
        }
        else {
            float m = (float)dx / (float)dy;
            int sign = Mathf.Sign(dy);
            if (sign == 0) return;
            for (int y = from.y; y != to.y + sign; y += sign) {
                Vector2Int v = new Vector2Int(Mathf.RoundToInt(m * (float)y) + translate.x, y + translate.y);
                _fov.SetCell(v.x, v.y, -1);
                if (!_localMap.IsOpen(v.x, v.y)) return;
            }
        }
    }
}