using Godot;
using System;

public class DebugCamera2D : KinematicBody2D {
	[Export] float Speed;
	Camera2D camera;
	[Export] float ZoomSpeed;
	float CurrSpeed;
	[Export] int DefaultZoom = 0;
	[Export] Vector2 DefaultPosition;
	[Export] bool LockPosition;

	public override void _Ready() {
		camera = (Camera2D)GetNode("Camera2D");
		CurrSpeed = Speed;
		for (int i = 0; i < DefaultZoom; ++i) {
			IncreaseZoom();
		}
		GlobalPosition = DefaultPosition;
	}

	public override void _Process(float delta) {
		var direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		if (!LockPosition) 
			MoveAndSlide(CurrSpeed * direction);
		if (Input.IsActionJustPressed("scroll_up")) {
			IncreaseZoom();
		}
		if (Input.IsActionJustPressed("scroll_down")) {
			DecreaseZoom();
		}
	}

	public void IncreaseZoom() {
		camera.Zoom = new Vector2(Mathf.Max(1, camera.Zoom.x + 1),
								  Mathf.Max(1, camera.Zoom.y + 1));
		CurrSpeed = Speed * camera.Zoom.x;
	}

	public void DecreaseZoom() {
		camera.Zoom = new Vector2(Mathf.Max(1, camera.Zoom.x - 1),
								  Mathf.Max(1, camera.Zoom.y - 1)); 
		CurrSpeed = Speed * camera.Zoom.x;
		
	}

	//  // Called every frame. 'delta' is the elapsed time since the previous frame.
	//  public override void _Process(float delta)
	//  {
	//      
	//  }
}
