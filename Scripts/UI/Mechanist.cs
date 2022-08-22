using System;
using Godot;
using System.Collections.Generic;
public class Mechanist : Exchange {
	protected override void SetItem() {
		Rarity rarity = Services.Instance.IconInstancer.Roll(Rarity.Singular);
		itemBeingSold = Services.Instance.IconInstancer
			.Select("*", "*", "*", "*", "Mechanist", rarity, -1);
	}

	protected override void AddToValue(IconData input) {
		if (input.HasMaterial("Metal")) {
			total += input.value * 2;
		}
		else { 
			total += input.value;
		}
	}
}
