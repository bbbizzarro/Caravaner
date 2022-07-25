using Godot;
using System;

public class TileRenderer : Node2D
{
	[Signal]
	delegate void SpriteSet(int id);

	public override void _Ready() {
	}

	public void SetSprite(int id) {
		EmitSignal(nameof(SpriteSet), id);
	}
}
