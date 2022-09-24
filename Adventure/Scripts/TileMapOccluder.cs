using Godot;
using Caravaner;

public class TileMapOccluder {
    TileMap _tileMap;
    LocalMap _localMap;
    int _pixelsPerUnit;

    public TileMapOccluder(TileMap tileMap, LocalMap localMap, int pixelsPerUnit) {
        _tileMap = tileMap;
        _localMap = localMap;
        _pixelsPerUnit = pixelsPerUnit;
    }

    public void Update() {
        for (int x = 0; x < _localMap.Width; ++x) {
            for (int y = 0; y < _localMap.Height; ++y) {
                Vector2Int pos = _localMap.ArrayToGrid(x, y);
                if (!_localMap.IsOpen(pos.x, pos.y)) {
                    _tileMap.SetCell(pos.x, pos.y, 0);//, autotileCoord:new Vector2(0, 0));
                }
            }
        }
    }
}