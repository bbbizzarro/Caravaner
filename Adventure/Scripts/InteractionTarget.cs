using Godot;

public delegate void InteractionTargetHandler(PlayerEntity playerEntity);
public class InteractionTarget : Area2D {
    public event InteractionTargetHandler InteractionEvent;

    public virtual void Interact(PlayerEntity playerEntity) {
        InteractionEvent?.Invoke(playerEntity);
    }
}