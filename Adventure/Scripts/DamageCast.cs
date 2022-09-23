using Godot;

public class DamageCast : Area2D {
    Timer _timer;
    int _damage;
    Area2D _origin;
    float _time;
    bool _oneHit;

    public override void _Ready() {
        _timer = (Timer)GetNode("Timer");
        _timer.Connect("timeout", this, nameof(OnTimerEnd));
        _timer.Start(_time);
        Connect("area_entered", this, nameof(Damage));
    }

    public DamageCast Init(int damage, float time, Area2D origin) {
        _damage = damage;
        _origin = origin;
        _time = time;
        return this;
    }

    public void SetOneHit() {
        _oneHit = true;
    }

    public void Damage(Area2D attackable) {
        if (_origin == attackable) return;
        GD.Print("Damage in damage cast");
        ((Attackable)attackable).Damage(-_damage);
        if (_oneHit) {
            QueueFree();
        }
    }

    public void OnTimerEnd() {
        QueueFree();
    }
}