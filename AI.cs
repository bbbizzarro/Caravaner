using Godot;
using System;

public class AI : KinematicBody2D {
	// Should wander around in a given square
	// Also want herd AI

	Animator animator;
	State state = State.Moving;
	Vector2 direction;
	Timer timer;
	RandomNumberGenerator rng;
	float stateTime = 2f;
	float speed = 50f;
	float SCALE = 64;
	Vector2 tile;

	// Integer value timers to time animation with movement stops?
	public override void _Ready() {
		rng = new RandomNumberGenerator();
		rng.Randomize();
		animator = (Animator)GetNode("EntityView");
		animator.SetSpeed(2f);
		timer = (Timer)GetNode("Timer");
		timer.Connect("timeout", this, nameof(UpdateState));
		UpdateState();
		timer.Start(stateTime);
		float x = Mathf.Round(GlobalPosition.x / SCALE) * SCALE;
		float y = Mathf.Round(GlobalPosition.y / SCALE) * SCALE;
		GD.Print(x, " ", y);
	}

	public override void _Process(float delta) {
		switch (state) {
			case State.Idle:
				HandleIdleState();
				break;
			case State.Moving:
				HandleMovingState();
				break;
			default:
				break;
		}
	}

	private void UpdateState() {
		switch (state) {
			case State.Idle:
				SetMovingState();
				break;
			case State.Moving:
				SetIdleState();
				break;
			default:
				break;
		}
		timer.Start(stateTime);
	}

	private void CalculateDirection() { 

	}

	private void SetIdleState() { 
		animator.Stop();
		state = State.Idle;
		direction = Vector2.Zero;
		stateTime = 2f;
	}
	private void SetMovingState() { 
		animator.Walk();
		state = State.Moving;
		direction = new Vector2(rng.RandfRange(-1, 1), rng.RandfRange(-1, 1));
		direction = direction.Normalized();
		//Vector2 target = GlobalPosition + SCALE * direction;
		//float x = Mathf.Round(target.x / SCALE) * SCALE;
		//float y = Mathf.Round(target.y / SCALE) * SCALE;
		//target = new Vector2(x, y);
		//GD.Print(target);
		//stateTime = target.Length() / speed;
	}

	private void HandleIdleState() {
	}

	private void HandleMovingState() {
		MoveAndSlide(speed * direction);
	}


	enum State { 
		Idle,
		Moving
	}

}
