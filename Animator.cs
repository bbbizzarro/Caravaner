using Godot;
using System;

public class Animator : Node2D {

	AnimationPlayer animationPlayer;
	string nextAnimation = "Idle";

	// We might want to dynamically generate tweens instead
	public override void _Ready() {
		animationPlayer = (AnimationPlayer)GetNode("AnimationPlayer");
		animationPlayer.Connect("animation_finished", this, nameof(UpdateAnimation));
		animationPlayer.PlaybackSpeed = 3f;
		//animationPlayer.Play("Walk");
	}

	public void SetSpeed(float speed) {
		animationPlayer.PlaybackSpeed = speed;
	}

	public void UpdateAnimation(string animation) { 
		if (nextAnimation != "") {
			animationPlayer.Play(nextAnimation);
		}
	}

	public void Walk() {
		if (animationPlayer.IsPlaying()) {
			nextAnimation = "Walk";
		}
		else { 
			animationPlayer.Play("Walk");
		}
	}
	public void Stop() {
		if (animationPlayer.IsPlaying()) { 
			nextAnimation = "Idle";
		}
		else { 
			animationPlayer.Play("Idle");
		}
	}


}
