using Godot;
public class Attackable : Area2D {
    Health _health;
    int _pixelsPerUnit;
    bool _isActive = true;

    public override void _Ready() {
    }

    public Attackable Init(Health health) {
        _health = health;
        return this;
    }

    public bool IsActive() {
        return _isActive;
    }

    public void Deactivate() {
        _isActive = false;
    }

    public void Damage(int amount) {
        if (_isActive)
            _health.Increment(amount);
    }
}