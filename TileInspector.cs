using Godot;
using System;

public class TileInspector : Node2D {

	Label label;

	public override void _Ready() {
		label = (Label)GetNode("Label");
	}


	private void OnDisplayItem(int x, int y, string title) {
		Position = new Vector2(x, y);
		label.Text = title;
	}

}


