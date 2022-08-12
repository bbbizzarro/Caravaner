using Godot;
using System;

public class Exchange : DropPoint {
	IconSpawner iconSpawner;
	AnimationPlayer animationPlayer;

	public override void _Ready() {
		base._Ready();
		iconSpawner = new IconSpawner();
		animationPlayer = (AnimationPlayer)GetNode("AnimationPlayer");
		animationPlayer.Play("StaticIdle");
	}

	public override bool Add(DragObject dragObject) {
		dragObject.Destroy();
		iconSpawner.SpawnGroup(GetParent(), 3, GlobalPosition);
		return true;
	}

	public override bool Remove(DragObject dragObject) {
		return false;
	}

	public override bool IsOpen() {
		return true;
	}

	protected override void Preview(bool preview) {
		if (preview) { 
			animationPlayer.Play("Squish");
		}
		else { 
			animationPlayer.Play("StaticIdle");
		}
	}
}
