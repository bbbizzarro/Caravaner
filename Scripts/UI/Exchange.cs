using Godot;
using System;
using System.Collections.Generic;

public class Exchange : MousePoint {
	IconSpawner iconSpawner;
	IconData itemBeingSold;
	int total = 0;

	public override void _Ready() {
		base._Ready();
		iconSpawner = new IconSpawner();
		var rng = new RandomNumberGenerator();
		rng.Randomize();
		itemBeingSold = Services.Instance.IconInstancer
			.GetRandom();
	}

	protected override void OnMousePress() {
		GD.Print(itemBeingSold.name, " ", itemBeingSold.value);
	}

	public override bool Add(DragObject dragObject) {
		IconData input = Services.Instance.IconInstancer
			.GetData(dragObject.GetItemName());
		total += input.value;
		dragObject.Destroy();
		if (total >= itemBeingSold.value) { 
			iconSpawner.Spawn(itemBeingSold.name, GlobalPosition);
			total = 0;
			return true;
		}
		return false;
	}

	public override bool IsOpen() {
		return true;
	}

}
