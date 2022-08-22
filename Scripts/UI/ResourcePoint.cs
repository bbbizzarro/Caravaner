using System;
using Godot;
using System.Collections.Generic;

public class ResourcePoint : DropPoint {
	private AnimationPlayer animationPlayer;
	private int integrity = 2;

	public override void _Ready() {
		base._Ready();
		animationPlayer = (AnimationPlayer)GetNode("AnimationPlayer");
	}

	protected override void OnMousePress() {
		animationPlayer.Play("StrongSquish");
		if (integrity <= 0 ) {
			List<IconData> icons = Services.Instance.IconInstancer
				.SelectMany(3, "Scrap|Food", "*", "*", "*", Rarity.Any, -1);
			Services.Instance.IconInstancer
				.SpawnGroup(GlobalPosition, icons);
			Destroy();
		}
		else {
			integrity -= 1;
		}
	}

	protected override void Preview(bool preview) {
		if (preview) { 
			//animationPlayer.Play("Squish");
			animationPlayer.Queue("Squish");
			//DragObject.SetMouseOffset(new Vector2(0, -64f));
		}
		else { 
			animationPlayer.Play("StaticIdle");
			//DragObject.SetMouseOffset(Vector2.Zero);
		}
	}
}
