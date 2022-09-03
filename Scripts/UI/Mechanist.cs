using System;
using Godot;
using System.Collections.Generic;
public class Mechanist : Exchange {

	public override void Initialize(Region region) {
		base.Initialize(region);
	}

	protected override void SetItem(string category) {
		itemBeingSold = Services.Instance.IconInstancer
			.Select("*", "*", "Mechanist", Rarity.Singular, -1);
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
