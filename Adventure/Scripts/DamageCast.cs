using Godot;

public class DamageCast : Area2D {
    Timer _timer;
    int _damage;
    Area2D _origin;

    public override void _Ready() {
        _timer = (Timer)GetNode("Timer");
        _timer.Connect("timeout", this, nameof(OnTimerEnd));
        _timer.Start(0.1f);
        Connect("area_entered", this, nameof(Damage));
    }

    public DamageCast Init(int damage, Area2D origin) {
        _damage = damage;
        _origin = origin;
        return this;
    }

    public void Damage(Area2D attackable) {
        if (_origin == attackable) return;
        GD.Print("Damage in damage cast");
        ((AttackableNode)attackable).Damage(-_damage);
    }

    public void OnTimerEnd() {
        QueueFree();
    }
}