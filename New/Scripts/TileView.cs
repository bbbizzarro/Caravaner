using Godot;
using System.Collections.Generic;

public class TileView : Sprite {

    GameWorld.Tile _tile;
    SpriteTable _spriteTable;

    public void Init(GameWorld.Tile tile, SpriteTable spriteTable) {
        _tile = tile;
        _tile.TileChangedEvent += UpdateSprite;
        _tile.TileDestroyedEvent += DestroyView;
        _spriteTable = spriteTable;
        UpdateSprite();
    }

    public void DestroyView() {
        QueueFree();
    }

    public void UpdateSprite() {
        Frame = _spriteTable.Get(_tile.Name);
    }
}