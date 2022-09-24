using Godot;
using Caravaner;

public class GroundRenderer {
    TileMap _tileMap;
    LocalMap _localMap;
    int _pixelsPerUnit;

    public GroundRenderer(TileMap groundTileMap, LocalMap localMap, int pixelsPerUnit) {
        _tileMap = groundTileMap;
        _localMap = localMap;
        _pixelsPerUnit = pixelsPerUnit;
    }

    public void Render() {
        for (int x = 0; x < _localMap.Width; ++x) {
            for (int y = 0; y < _localMap.Height; ++y) {
                Vector2Int pos = _localMap.ArrayToGrid(x, y);
                _tileMap.SetCell(pos.x, pos.y, 2);//, autotileCoord:new Vector2(0, 0));
            }
        }
    }
}