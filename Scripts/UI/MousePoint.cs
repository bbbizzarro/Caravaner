using System;
using Godot;
public class MousePoint : DropPoint{

	protected Sprite sprite;
	protected AnimationPlayer animationPlayer;

    public override void _Ready() {
        base._Ready();
		sprite = (Sprite)GetNode("Sprite");
		animationPlayer = (AnimationPlayer)GetNode("AnimationPlayer");
		animationPlayer.Play("StaticIdle");
    }

    protected override void Preview(bool preview) {
		if (preview) { 
			animationPlayer.Play("Squish");
			DragObject.SetMouseOffset(new Vector2(0, -64f));
		}
		else { 
			animationPlayer.Play("StaticIdle");
			DragObject.SetMouseOffset(Vector2.Zero);
		}
	}
}
