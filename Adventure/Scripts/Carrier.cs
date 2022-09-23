using Godot;
using System;

public class Carrier : Node2D {
    const int OnSprite = 120;
    const int OffSprite = 19;
    float _pixelsPerUnit;
    Sprite _sprite;
    Node _main;
    PackedScene ItemPackedScene = (PackedScene)ResourceLoader.Load("res://Adventure/Scenes/Item.tscn");
    bool _isSet;
    Node2D _parent;
    float _throwStrength;
    const float _throwStrengthSpeed = 1f;
    Attackable _origin;

    public Carrier Init(Node2D parent, float pixelsPerUnit, Attackable origin) {
        _pixelsPerUnit = pixelsPerUnit;
        _sprite = (Sprite)GetNode("Sprite");
        _sprite.Frame = OffSprite;
        _parent = parent;
        _origin = origin;
        Position = parent.Position + _pixelsPerUnit * new Vector2(0, -1f);
        return this;
    }

    public void IncrementThrowStrength(float delta) {
        _throwStrength = Mathf.Clamp(_throwStrengthSpeed * _throwStrength + delta, 0, 1);
    }

    public void UnSet(Vector2 direction, float magnitude) {
        _sprite.Frame = OffSprite;
        var item = ((ItemObject)ItemPackedScene.Instance()).Init(direction, magnitude, 0.2f, _origin);
        _parent.GetParent().AddChild(item);
        item.GlobalPosition = _parent.GlobalPosition;
        _isSet = false;
        _throwStrength = 0;
    }

    public void Set() {
        _sprite.Frame = OnSprite;
        _isSet = true;
    }

    public bool IsSet() {
        return _isSet;
    }
}
