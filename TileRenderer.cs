using Godot;
using System;

public class TileRenderer : Node2D
{
	[Signal]
	delegate void SpriteSet(int id);

	public void Initialize(int xPos, int yPos, int spriteID) {
		Position = new Vector2(xPos, yPos);
		SetSprite(spriteID);
	}

	public void SetSprite(int id) {
		EmitSignal(nameof(SpriteSet), id);
	}
}
