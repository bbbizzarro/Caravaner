using Godot;
using System;
using System.Collections.Generic;

public class Exchange : MousePoint {
	IconSpawner iconSpawner;
	protected IconData itemBeingSold;
	protected int total = 0;
	Label label;
	Timer timer;
	RandomNumberGenerator rng;

	public override void _Ready() {
		base._Ready();
		label = (Label)GetNode("Sprite/Label");
		timer = (Timer)GetNode("Timer");
		timer.Connect("timeout", this, nameof(ResetClickPreview));
		iconSpawner = new IconSpawner();
		rng = new RandomNumberGenerator();
		rng.Randomize();
		SetItem();
	}

	protected virtual void SetItem() { 
		itemBeingSold = Services.Instance.IconInstancer
			.GetRandom();
	}

	protected virtual void AddToValue(IconData input) { 
		total += input.value;
	}

	protected override void OnMousePress() {
		if (itemBeingSold == null) return;
		timer.Start(2f);
		label.Text = 
			String.Format("{0} ({1}/{2})", itemBeingSold.name, total, itemBeingSold.value);
	}

	private void ResetClickPreview() {
		label.Text = "";
	}

	public override bool Add(DragObject dragObject) {
		if (itemBeingSold == null) return false;
		IconData input = Services.Instance.IconInstancer
			.GetData(dragObject.GetItemName());
		dragObject.Destroy();
		AddToValue(input);
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
