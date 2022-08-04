using Godot;
using System;
using System.Collections.Generic;

public class Hotbar : HBoxContainer {
	private int count = 5;
	private List<HotbarItem> itemViews = new List<HotbarItem>();

	public override void _Ready() {
		foreach (HotbarItem n in GetChildren()) {
			itemViews.Add(n);
		}
	}

	public void OnPlayerContainerUpdated(IEnumerable<Item> items) {
		int index = 0;
		foreach (Item i in items) {
			if (index >= count) return;
			itemViews[index].Initialize(i);
			index += 1;
		}
	}

}
