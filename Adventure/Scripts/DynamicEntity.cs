using Godot;

public class DynamicEntity : KinematicBody2D {
    protected EntityService _entityService;
    protected int _pixelsPerUnit;
    float _movementSpeed;

    public virtual void Init(int pixelsPerUnit, float movementSpeed, EntityService entityService) {
        _pixelsPerUnit = pixelsPerUnit;
        _movementSpeed = movementSpeed;
        _entityService = entityService;
    }

    public void Destroy() {
        _entityService.Remove(this);
        QueueFree();
    }

    public void Move(Vector2 direction) {
		MoveAndSlide(_pixelsPerUnit * _movementSpeed * direction);
    }
}