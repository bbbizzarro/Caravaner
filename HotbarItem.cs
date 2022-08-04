using Godot;
using System;

public class HotbarItem : TextureRect {
	Label name;
	Label reqs;

	public override void _Ready() {
		name = (Label)GetNode("Name");
		reqs = (Label)GetNode("Reqs");
	}
	public void Initialize(Item item) {
		this.name.Text = item.name;
		this.reqs.Text = String.Format("{0}:{1}", item.req, item.amount);
	}
}
