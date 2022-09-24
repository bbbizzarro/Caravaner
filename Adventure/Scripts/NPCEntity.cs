using Godot;

public class NPCEntity : DynamicEntity {
    Health _health;
    Attackable _attackable;


    public override void Init(int pixelsPerUnit, float movementSpeed, EntityService entityService) {
        base.Init(pixelsPerUnit, movementSpeed, entityService);
        _health = new Health(10);
        _health.OnHealthZero += Destroy;
        _attackable = ((Attackable)GetNode("Attackable")).Init(_health);
    }

    public override void _Process(float delta) {
        MoveTowardPlayer();
    }

    private void MoveTowardPlayer() {
        if (_entityService == null || _entityService.GetPlayer() == null) return;
        Node2D node = _entityService.GetPlayer();
        var direction = (node.GlobalPosition - GlobalPosition).Normalized();
        Move(direction);
    }
}