using Godot;
using System.Collections.Generic;

public class TileView : Sprite {

    GameWorld.Tile _tile;
    SpriteTable _spriteTable;

    public void Init(GameWorld.Tile tile, SpriteTable spriteTable) {
        _tile = tile;
        _tile.TileChangedEvent += UpdateSprite;
        _spriteTable = spriteTable;
        UpdateSprite();
    }

    public void UpdateSprite() {
        GameWorld.Prop prop = _tile.GetTopProp();
        if (prop != null) Render(prop.Name);
        else Render("Dirt");
    }

    private void Render(string id) {
        Frame = _spriteTable.Get(id);
    }
}