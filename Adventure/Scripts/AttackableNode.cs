using Godot;
public class AttackableNode : Area2D {
    Health _health;
    int _pixelsPerUnit;

    public override void _Ready() {
    }

    public AttackableNode Init(Health health) {
        _health = health;
        return this;
    }

    public void Damage(int amount) {
        GD.Print(amount);
        _health.Increment(amount);
    }
}