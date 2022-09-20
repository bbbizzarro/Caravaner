using Godot;
using System;

public class Interactable : Area2D {
    public static Interactable active;

    private int _energyCost = 10;
    Entity _entity;
    WorldMapRenderer _renderer;
    Sprite _sprite;

    public void Init(Entity entity, WorldMapRenderer renderer) {
        _renderer = renderer;
        _entity = entity;
        _entity.EntityDestroyedEvent += HandleEntityDestroyed;
        GlobalPosition = _renderer.GridToWorld(entity.Position);
        _sprite.Texture = renderer.GetSprite(entity.Sprite);
    }

    public override void _Ready() {
        Connect("body_entered", this, nameof(SetAsActive));
        Connect("body_exited", this, nameof(SetInactive));
        _sprite = (Sprite)GetNode("Sprite");
    }

    public void SetAsActive(Node body) {
        if (body.IsInGroup("Player")) {
            active = this;
        }
    }

    public void SetInactive(Node body) {
        if (active == this) active = null;
    }

    public void HandleEntityDestroyed(Entity entity) {
        QueueFree();
    }

    public virtual void Interact() {

        Player p = ((Player)GetNode("/root/Main/Player"));
        p.SimulateAnimation(0.1f);
        _entity.Interact(p, Services.Instance.PlayerData);
    }
}
