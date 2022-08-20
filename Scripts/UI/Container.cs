using Godot;
using System;

public class Container : MousePoint {
	InventoryUI inventory;
	bool inventoryIsEnabled = false;

	public override void _Ready() {
		base._Ready();
		inventory = (InventoryUI)GetNode("IconContainer");
		inventory.Enable(false);
	}

	protected override void OnMousePress() {
		GD.Print("Press!");
		inventoryIsEnabled = !inventoryIsEnabled;
		inventory.Enable(inventoryIsEnabled);
	}
}
