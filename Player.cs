using Godot;
using System;

public class Player : KinematicBody2D {

	private float speed = 2f;
	private Vector2 direction = Vector2.Zero;
	private float MAX_FRAME_RATE = 60f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta) {
		direction = Input.GetVector("ui_right", "ui_left", "ui_up", "ui_down");
		MoveAndCollide(MAX_FRAME_RATE * delta * speed * direction);
	}
}
