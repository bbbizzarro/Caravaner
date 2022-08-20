using System;
using Godot;
using System.Collections.Generic;
public class Mechanist : Exchange {
	protected override void SetItem() {
		itemBeingSold =
			Services.Instance.IconInstancer.GetRandomIconFromCategory("Advanced Mechanics", 100);
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
