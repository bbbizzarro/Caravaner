using Godot;
using System;

public class Camera : Camera2D {
	ShaderMaterial postProcMat;
	[Export] float speed;
	float amt = 0f;

	public override void _Ready() {
		postProcMat = (ShaderMaterial)((CanvasItem)GetNode("CanvasLayer/ColorRect")).Material;
	}

	public override void _Process(float delta) {
		if (Input.IsActionPressed("ui_increase")) {
			amt = Mathf.Clamp(amt + delta, 0, 1);
			postProcMat.SetShaderParam("transition_amount", amt);
		}
		if (Input.IsActionPressed("ui_decrease")) { 
			amt = Mathf.Clamp(amt - delta, 0, 1);
			postProcMat.SetShaderParam("transition_amount", amt);
		}
	}
}
