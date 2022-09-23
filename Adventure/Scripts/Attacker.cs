using Godot;

public class Attacker : Node {
    Node2D _parent;
    Area2D _origin;
    PackedScene _damageCastScene = (PackedScene)ResourceLoader.Load("res://Adventure/Scenes/DamageCast.tscn");
    AnimationPlayer _animationPlayer;
    float _pixelsPerUnit;

    public Attacker Init(Node2D parent, float pixelsPerUnit, Area2D origin, AnimationPlayer animationPlayer) {
        _parent = parent;
        _origin = origin;
        _pixelsPerUnit = pixelsPerUnit;
        _animationPlayer = animationPlayer;
        return this;
    }

    public void Attack(Vector2 direction) {
        DamageCast damageCast = ((DamageCast)_damageCastScene.Instance()).Init(10, 0.1f, _origin);
        _parent.AddChild(damageCast);
        damageCast.Position = _pixelsPerUnit * direction;
    }

    public void Execute() {
    }

}

public class Attack {


    public void Execute() {
    }
}