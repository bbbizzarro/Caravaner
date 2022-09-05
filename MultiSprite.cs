using Godot;
using System;
using System.Collections.Generic;

public class MultiSprite : Node2D, RegionObject {

    private SpriteDistributor spriteDistributor;

    public override void _Ready() {
        spriteDistributor = (SpriteDistributor)GetNode("SpriteDistributor");
    }

    private List<Texture> GetTextures() {
        var textures = new List<Texture>();
        //var textureNames = new List<string>() {"atex_001", "atex_002", "atex_003", "atex_004", "atex_005", "atex_006"};
        //foreach (var name in textureNames) {
        //    textures.Add(Services.Instance.SpriteDB.GetAtlasTexture(name));
        //}
        for (int i = 1; i < 4; ++i) {
            if (i == 9) continue;
            textures.Add(Services.Instance.SpriteDB.GetAtlasTexture("atex_" + i.ToString("D3")));
        }
        return textures;
    }

    public void Initialize(Region region) {
        spriteDistributor.Initialize(GetParent(), GetTextures());
    }
}
