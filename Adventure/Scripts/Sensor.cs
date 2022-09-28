using Godot;
using System;

public class Sensor : Area2D {
    public event EventHandler<string> AddItemEvent;

    public void AddItem(object sender, string ItemID) {
        AddItemEvent?.Invoke(sender, ItemID);
    }
}
