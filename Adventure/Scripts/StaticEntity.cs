using Godot;

public class StaticEntity : Node2D, IHasHealth {
    Health _health;
    Attackable _attackable;
    const int LiveFrame = 348;
    const int DeadFrame = 48;
    Sprite _sprite;
    CollisionShape2D _collisionShape;

    public override void _Ready() {
        _health = new Health(10);
        _health.OnHealthZero += Destroy;
        _attackable = ((Attackable)GetNode("AttackableNode")).Init(_health);
        _sprite = (Sprite)GetNode("Sprite");
        _sprite.Frame = LiveFrame;
        _collisionShape = (CollisionShape2D)GetNode("CollisionShape2D");
    }

    public void Destroy() {
        _sprite.Frame = DeadFrame;
        _attackable.Deactivate();
        _collisionShape.SetDeferred("disabled", true);
        //QueueFree();
    }

    public Health GetHealth() {
        return _health;
    }

}
