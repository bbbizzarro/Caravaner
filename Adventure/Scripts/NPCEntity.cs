using Godot;

public class NPCEntity : DynamicEntity {

    public override void _Process(float delta) {
        MoveTowardPlayer();
    }

    private void MoveTowardPlayer() {
        Node2D node = _entityService.GetPlayer();
        var direction = (node.GlobalPosition - GlobalPosition).Normalized();
        Move(direction);
    }
}