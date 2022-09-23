using Godot;
using Caravaner;

public class PlayerEntity : DynamicEntity, IHasHealth {
    Health _health;
    AnimationPlayer _animationPlayer;
    Attacker _attacker;
    Attackable _attackable;
    Facing _facing;
    Interactor _interactor;
    public Carrier Carrier {private set; get; }

    public Health GetHealth() {
        return _health;
    }

    public override void Init(int pixelsPerUnit, float movementSpeed, EntityService entityService) {
        base.Init(pixelsPerUnit, movementSpeed, entityService);
        _entityService = entityService;
        _entityService.Add(this);

        _health = new Health(100);
        _animationPlayer = ((AnimationPlayer)GetNode("AnimationPlayer"));
        _attackable = ((Attackable)GetNode("Attackable")).Init(_health);
        _attacker = ((Attacker)GetNode("Attacker")).Init(this, _pixelsPerUnit, _attackable, _animationPlayer);
        _facing = new Facing();
        _interactor = ((Interactor)GetNode("Interactor")).Init(this);
        Carrier = ((Carrier)GetNode("Carrier")).Init(this, pixelsPerUnit, _attackable);
    }

    public override void _Process(float delta) {
        if (!_animationPlayer.IsPlaying()) {
            ProcessInput(delta);
        }
    }

    private void ProcessInput(float delta) {
		var direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
        _facing.Update(direction);
        Move(direction);
        if (Input.IsActionJustPressed("Attack")) {
            if (Carrier.IsSet()) {
            }
            else {
                _attacker.Attack(_facing.Curr);
                _animationPlayer.Play("EntityAnimation");
            }
        }
        if (Input.IsActionPressed("Attack")) {
            if (Carrier.IsSet()) {
                Carrier.IncrementThrowStrength(delta);
            }
        }
        if (Input.IsActionJustReleased("Attack")) {
            if (Carrier.IsSet()) {
                Carrier.UnSet(_facing.Curr, _pixelsPerUnit * 25f);
            }
        }
        if (Input.IsActionJustPressed("interact")) {
            _interactor.Interact();
        }
    }
}