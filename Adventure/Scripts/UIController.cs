using Godot;
using System;
using System.Collections.Generic;

public class UIController : CanvasLayer {

    Label _inventoryLabel;

    public override void _Ready() {
        _inventoryLabel = (Label)GetNode("HBoxContainer/Inventory/Text");
    }

    public void UpdatePlayerInventory(EntityInventory inventory) {
        string sm = "";
        foreach (var i in inventory.GetItemIds()) {
            sm += String.Format("[{0}](x{1})", i, inventory.GetCount(i));
        }
        _inventoryLabel.Text = sm;
    }
}
