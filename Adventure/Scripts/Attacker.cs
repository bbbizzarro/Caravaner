using Godot;

public class Attacker {
    Node _parent;
    Area2D _origin;
    PackedScene _damageCastScene = (PackedScene)ResourceLoader.Load("res://Adventure/Scenes/DamageCast.tscn");

    public Attacker(Node parent, Area2D origin) {
        _parent = parent;
        _origin = origin;
    }

    public void Attack(Vector2 point) {
        DamageCast damageCast = ((DamageCast)_damageCastScene.Instance()).Init(10, _origin);
        _parent.AddChild(damageCast);
        damageCast.GlobalPosition = point;
    }
}