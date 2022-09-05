using Godot;
using System;
using Caravaner;
using System.Collections.Generic;

public class SpriteDistributor : Node2D {
    private PackedScene SpritePackedScene = 
        (PackedScene)ResourceLoader.Load("res://SpriteScene.tscn");
    private RandomNumberGenerator rng = new RandomNumberGenerator();
    private Sprite[] sprites;
    private const int NumberOfSprites = 2;
    private const int WorldScale = 64;

    public void Initialize(Node parentNode, List<Texture> textures) {
        sprites = new Sprite[NumberOfSprites];
        rng.Randomize();
        for (int i = 0; i < NumberOfSprites; ++i) {
            Sprite s = (Sprite)SpritePackedScene.Instance();
            var tex = textures[rng.RandiRange(0, textures.Count - 1)];
            s.Texture = tex;
            s.Offset = new Vector2(0, -tex.GetHeight() / 2);
            parentNode.CallDeferred("add_child", s);
            //GetParent().AddChild(s);
            sprites[i] = s;
        }
        ArrangeSprites();
    }

    public void Initialize(Node parentNode, params Texture[] textures) {
        sprites = new Sprite[NumberOfSprites];
        for (int i = 0; i < NumberOfSprites; ++i) {
            Sprite s = (Sprite)SpritePackedScene.Instance();
            var tex = textures[rng.RandiRange(0, textures.Length - 1)];
            s.Texture = tex;
            s.Offset = new Vector2(0, -tex.GetHeight() / 2);
            parentNode.CallDeferred("add_child", s);
            //GetParent().AddChild(s);
            sprites[i] = s;
        }
        rng.Randomize();
        ArrangeSprites();
    }

    private void ArrangeSprites() {
        // y = -0.25f, -0.75f
        // x = 0.25f  or 0.75f
        Vector2 pos = new Vector2(GlobalPosition.x - 0.5f * WorldScale, GlobalPosition.y + 0.5f * WorldScale);
        sprites[0].GlobalPosition = GlobalPosition; 

        //sprites[0].GlobalPosition = pos + WorldScale * new Vector2(0.30f, -0.25f);
        //sprites[1].GlobalPosition = pos + WorldScale * new Vector2(0.70f, -0.75f);
        /*
        if (rng.Randf() > 0.5f) {
            sprites[0].GlobalPosition = pos + WorldScale * new Vector2(0.30f, -0.25f);
            sprites[1].GlobalPosition = pos + WorldScale * new Vector2(0.70f, -0.75f);
        }
        else {
            sprites[0].GlobalPosition = pos + WorldScale * new Vector2(0.70f, -0.25f);
            sprites[1].GlobalPosition = pos + WorldScale * new Vector2(0.30f, -0.75f);
        }
        */
    }
}
