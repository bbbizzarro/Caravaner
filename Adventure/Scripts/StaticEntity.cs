using Godot;
using Caravaner;

public class StaticEntity : Node2D, IHasHealth {
    Health _health;
    Attackable _attackable;
    [Export] int LiveFrame = 348;
    [Export] int DeadFrame = 48;
    [Export] int TotalHealth = 10;
    Sprite _sprite;
    CollisionShape2D _collisionShape;
    LocalMap _localMap;
    Vector2Int _position;
    int _pixelsPerUnit;

    public override void _Ready() {
        _health = new Health(TotalHealth);
        _health.OnHealthZero += Destroy;
        _attackable = ((Attackable)GetNode("AttackableNode")).Init(_health);
        _sprite = (Sprite)GetNode("Sprite");
        //_sprite.Frame = LiveFrame;
        _collisionShape = (CollisionShape2D)GetNode("CollisionShape2D");
    }

    public StaticEntity Init(Vector2Int position, LocalMap localMap, int pixelsPerUnit) {
        _localMap = localMap;
        _position = position;
            _localMap.Set(_position.x, _position.y, false);
        _pixelsPerUnit = pixelsPerUnit;
        GlobalPosition = new Vector2(position.x * pixelsPerUnit, position.y * pixelsPerUnit);
        return this;
    }

    public void Destroy() {
        _sprite.Frame = DeadFrame;
        _attackable.Deactivate();
        _collisionShape.SetDeferred("disabled", true);
        if (_localMap != null) {
            _localMap.Set(_position.x, _position.y, true);
        }
        //QueueFree();
    }

    public Health GetHealth() {
        return _health;
    }

}
