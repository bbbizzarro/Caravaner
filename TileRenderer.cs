using Godot;
using System;
using Caravaner;

public class TileRenderer : Node2D, IActive
{
	[Signal] delegate void SpriteSet(int id);
	[Signal] delegate void NodeActiveSet(TileRenderer tileRenderer, bool isActive);

	public void Initialize(int xPos, int yPos, int spriteID) {
		Position = new Vector2(xPos, yPos);
		SetSprite(spriteID);
	}

	public void SetSprite(int id) {
		EmitSignal(nameof(SpriteSet), id);
	}

	public void SetActive(bool active) {
		if (active) {
			Visible = true;
		}
		else {
			Visible = false;
			Position = new Vector2(-5f * 64f, -5f * 64f);
		}
		//EmitSignal(nameof(NodeActiveSet), this, active);
	}
}
