using Godot;

public class StaticEntity : Node2D, IHasHealth {
    Health _health;
    AttackableNode _attackable;
    public override void _Ready() {
        _health = new Health(10);
        _health.OnHealthZero += Destroy;
        _attackable = ((AttackableNode)GetNode("AttackableNode")).Init(_health);
    }

    public void Destroy() {
        QueueFree();
    }

    public Health GetHealth() {
        return _health;
    }

}
