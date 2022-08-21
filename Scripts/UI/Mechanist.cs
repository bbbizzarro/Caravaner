using System;
using Godot;
using System.Collections.Generic;
public class Mechanist : Exchange {
	protected override void SetItem() {
		itemBeingSold = Services.Instance.IconInstancer
			.Select("Advanced Mechanics", "*", "*", "*", Rarity.Any, -1);
	}

	protected override void AddToValue(IconData input) {
		if (input.InCategory("Metal")) {
			total += input.value * 2;
		}
		else { 
			total += input.value;
		}
	}
}
