using Godot;
using Caravaner;

public class PlayerEntity : DynamicEntity, IHasHealth {
    Health _health;
    Attacker _attacker;
    AttackableNode _attackable;
    Vector2 facing;

    public override void _Ready() {
        _health = new Health(100);
        _attackable = ((AttackableNode)GetNode("AttackableNode")).Init(_health);
        _attacker = new Attacker(GetParent(), _attackable);
        facing = new Vector2(0, -1);
    }

    public Health GetHealth() {
        return _health;
    }

    public override void Init(int pixelsPerUnit, float movementSpeed, EntityService entityService) {
        base.Init(pixelsPerUnit, movementSpeed, entityService);
        _entityService = entityService;
        _entityService.Add(this);
    }
    public override void _Process(float delta) {
		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
        if (direction != Vector2.Zero) facing = direction;
        Move(direction);
        if (Input.IsActionJustPressed("Attack")) {
            _attacker.Attack(GlobalPosition + _pixelsPerUnit * facing);
        }
    }
}