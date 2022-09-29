using Godot;

public class NPCEntity : DynamicEntity {
    Attackable _attackable;
    Sensor _sensor;

    Health _health;
    State _currentState;
    Node2D _target;
    Label _label;
    AnimationPlayer _animationPlayer;
    Attacker _attacker;

    public enum State {
        Idle,
        MoveTowardTarget,
        TelegraphAttack,
        Attack,
        Recover
    }

    public override void Init(int pixelsPerUnit, float movementSpeed, EntityService entityService) {
        base.Init(pixelsPerUnit, movementSpeed, entityService);
        _health = new Health(10);
        _health.OnHealthZero += Destroy;
        _attackable = ((Attackable)GetNode("Attackable")).Init(_health);
        _sensor = (Sensor)GetNode("Sensor");
        _sensor.Connect("area_entered", this, nameof(TargetDetected));
        _label = (Label)GetNode("Label");
        _animationPlayer = (AnimationPlayer)GetNode("AnimationPlayer");
        _attacker = ((Attacker)GetNode("Attacker")).Init(this, Globals.PixelsPerUnit, _attackable, _animationPlayer);
    }

    public override void _Process(float delta) {
        _label.Text = _currentState.ToString();
        HandleState(delta);
    }

    public void TargetDetected(Area2D target) {
        if (target.IsInGroup("Player")) {
            _target = target;
        }
    }

    private void MoveTowardPlayer() {
        //if (_entityService == null || _entityService.GetPlayer() == null) return;
        //Node2D node = _entityService.GetPlayer();
        //var direction = (node.GlobalPosition - GlobalPosition).Normalized();
        //Move(direction);
    }

    private void Idle(float delta) {
        if (_target != null) {
            _currentState = State.MoveTowardTarget;
        }
    }

    private void MoveTowardTarget(float delta) {
        if (_target != null) {
            Move(DirectionToTarget());
        }
        float dist = DistanceToTarget();

        if (dist > Globals.PixelsPerUnit * 10) {
            _currentState = State.Idle;
            _target = null;
        }
        else if (dist < Globals.PixelsPerUnit * 1.2f) {
            _animationPlayer.Play("Attack");
            _currentState = State.TelegraphAttack;
        }
    }

    private void TelegraphAttack(float delta) {
        //if (!_animationPlayer.IsPlaying()) {
        //    _attacker.Attack(DirectionToTarget());
        //    _currentState = State.MoveTowardTarget;
        //}
    }

    private void Recover(float delta) {
        _animationPlayer.Play("Recover");
    }

    private void AttackTarget() {
        _attacker.Attack(DirectionToTarget());
    }

    public void SwitchState(State state) {
        _currentState = state;
    }


    private float DistanceToTarget() {
        return (_target.GlobalPosition - GlobalPosition).Length();
    }

    private Vector2 DirectionToTarget() {
        return (_target.GlobalPosition - GlobalPosition).Normalized();
    }

    private void HandleState(float delta) {
        switch (_currentState) {
            case State.Idle: Idle(delta); return;
            case State.MoveTowardTarget: MoveTowardTarget(delta); return;
            case State.TelegraphAttack: TelegraphAttack(delta); return;
            case State.Recover: Recover(delta); return;
            default: return;
        }
    }
}