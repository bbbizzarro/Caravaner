using Godot;
using System;

public class ItemEntity : InteractionTarget {

    public override void Interact(PlayerEntity playerEntity) {
        playerEntity.Carrier.Set();
        QueueFree();
    }
}