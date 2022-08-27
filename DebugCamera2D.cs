using Godot;
using System;

public class DebugCamera2D : KinematicBody2D {
	[Export] float Speed;
	Camera2D camera;
	[Export] float ZoomSpeed;
	float CurrSpeed;

	public override void _Ready() {
		camera = (Camera2D)GetNode("Camera2D");
		CurrSpeed = Speed;
	}

	public override void _Process(float delta) {
		var direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		MoveAndSlide(CurrSpeed * direction);
		if (Input.IsActionJustPressed("scroll_up")) {
			camera.Zoom = new Vector2(Mathf.Max(1, camera.Zoom.x + 1),
									  Mathf.Max(1, camera.Zoom.y + 1));
			CurrSpeed = Speed * camera.Zoom.x;
		}
		if (Input.IsActionJustPressed("scroll_down")) {
			camera.Zoom = new Vector2(Mathf.Max(1, camera.Zoom.x - 1),
									  Mathf.Max(1, camera.Zoom.y - 1)); 
			CurrSpeed = Speed * camera.Zoom.x;
		}
	}


	//  // Called every frame. 'delta' is the elapsed time since the previous frame.
	//  public override void _Process(float delta)
	//  {
	//      
	//  }
}
