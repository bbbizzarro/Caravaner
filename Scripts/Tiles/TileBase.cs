using System;
using Godot;
public class TileBase : DropPoint {
	private IconSpawner iconSpawner;
	private AnimationPlayer animationPlayer;
	private int integrity = 2;

	public override void _Ready() {
		base._Ready();
		iconSpawner = new IconSpawner();
		animationPlayer = (AnimationPlayer)GetNode("AnimationPlayer");
	}

	protected override void OnMousePress() {
		animationPlayer.Play("StrongSquish");
		if (integrity <= 0 ) { 
			iconSpawner.SpawnGroup(3, GlobalPosition);
			Destroy();
		}
		else {
			integrity -= 1;
		}
	}

	protected override void Preview(bool preview) {
		if (preview) { 
			animationPlayer.Queue("Squish");
			//DragObject.SetMouseOffset(new Vector2(0, -64f));
		}
		else { 
			animationPlayer.Play("StaticIdle");
			//DragObject.SetMouseOffset(Vector2.Zero);
		}
	}
}
