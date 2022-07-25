using Godot;
using System;

public class SpriteController : Sprite {
	private void _on_Tile_SpriteSet(int id) {
		Frame = id;
	}
}
