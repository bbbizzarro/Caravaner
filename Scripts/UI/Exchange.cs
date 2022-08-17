using Godot;
using System;

public class Exchange : MousePoint {
	IconSpawner iconSpawner;

	public override void _Ready() {
		base._Ready();
		iconSpawner = new IconSpawner();
	}

	public override bool Add(DragObject dragObject) {
		dragObject.Destroy();
		iconSpawner.SpawnGroup(3, GlobalPosition);
		return true;
	}

	public override bool IsOpen() {
		return true;
	}

}
