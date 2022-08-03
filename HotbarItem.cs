using Godot;
using System;

public class HotbarItem : TextureRect {
	Label name;

	public override void _Ready() {
		name = (Label)GetNode("Name");
	}
	public void Initialize(string name) {
		this.name.Text = name;
	}
}
