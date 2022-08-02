using Godot;
using System;

public class Selector : Node2D {
	private float scale = 64f;

	public override void _Process(float delta) {
		Position = RoundToNearestGrid(GetGlobalMousePosition());
	}

	private Vector2 RoundToNearestGrid(Vector2 pos) {
		return new Vector2(Mathf.Round((float)pos.x / scale)*scale,
						   Mathf.Round((float)pos.y / scale)*scale);
	}
}
