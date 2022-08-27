using Godot;
using System;

public class Camera : Camera2D {
	ShaderMaterial postProcMat;
	[Export] float transitionTime;
	float amt = 0f;
	Timer timer;
	[Export] float lengthOfDayInSeconds;
	[Export] float lengthOfNightInSeconds;
	Tween tween;
	public float nightAmount;
	bool isDay;
	[Export] bool dayNightCycleOn;
	//[Export] bool pixelSnap;

	public override void _Ready() {
		postProcMat = (ShaderMaterial)((CanvasItem)GetNode("CanvasLayer/ColorRect")).Material;
		timer = (Timer)GetNode("Timer");
		if (dayNightCycleOn) 
			timer.Start(lengthOfDayInSeconds);
		tween = (Tween)GetNode("Tween");
		isDay = true;
		timer.Connect("timeout", this, nameof(OnTimerUp));
		//!!!
	}

	private void DayToNight() {
		tween.InterpolateProperty(this, "nightAmount", 0, 1, transitionTime);
		tween.Start();
	}
	private void NightToDay() {
		tween.InterpolateProperty(this, "nightAmount", 1, 0, transitionTime);
		tween.Start();
	}

	public void OnTimerUp() { 
		if (isDay) {
			DayToNight();
			timer.Start(lengthOfNightInSeconds);
			isDay = false;
		}
		else {
			NightToDay();
			timer.Start(lengthOfDayInSeconds);
			isDay = true;
		}
	}

	private void RoundPositionToNearestInt() {
		float x = Mathf.Round(Position.x) - Position.x;
		float y = Mathf.Round(Position.x) - Position.y;
		Offset = new Vector2(x, y);
	}

	public override void _Process(float delta) {
		if (tween.IsActive()) { 
			postProcMat.SetShaderParam("transition_amount", nightAmount);
		}
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
