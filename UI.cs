using Godot;
using System;

public class UI : Node2D {
	private World world;

	public void Initialize(World world) {
		this.world = world;
	}
}
