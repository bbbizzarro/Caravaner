using Godot;
using Godot.Collections;
using System;

public class Player : KinematicBody2D, ISavable {

	private float speed = 100f;
	private Vector2 direction = Vector2.Zero;
	//private readonly float MAX_FRAME_RATE = 60f;

	[Signal]
	delegate void Test();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		EmitSignal(nameof(Test));
	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta) {
		direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		//MoveAndCollide(MAX_FRAME_RATE * delta * speed * direction);
		MoveAndSlide(speed * direction);
	}

	public Godot.Collections.Dictionary<string, object> Save() {
		return new Godot.Collections.Dictionary<string, object>() {
			{ "Filename", Filename },
			{ "Parent", GetParent().GetPath() },
			{ "PosX", Position.x }, 
			{ "PosY", Position.y }
		};
	}

	public void Load(Dictionary<string, object> data) {
		if (data.ContainsKey("PosX") && data.ContainsKey("PosY")) {
			Position = new Vector2((float)data["PosX"], (float)data["PosY"]);
		}
	}
}
