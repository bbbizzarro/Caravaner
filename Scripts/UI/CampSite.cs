using Godot;
using System;
using System.Collections.Generic;

public class CampSite : MousePoint {
	[Export] int onFrame;
	[Export] int offFrame;
	State state;
	IconSpawner iconSpawner = new IconSpawner();

	// TODO
	// How do we handle partial state changes
	// OOP for state machines i.e. composite state machines
	// So we need a way to take multiple state machines
	// and compose them

	public override void _Ready() {
		base._Ready();
		sprite.Frame = offFrame;
		var sOn = new State("On");
		var sOff = new State("Off");

		sOff.AddTransition("Paper", "Charcoal", sOn, 50, 0);
		sOff.AddTransition("Paper", "", sOn, 50, 0);

		sOn.AddTransition("Raw Food", "[Cooked]", sOn, 90, 0);
		sOn.AddTransition("Raw Food", "Charcoal", sOn, 10, 0);
		sOn.AddTransition("Paper", "Charcoal", sOn, 100, 0);
		sOn.AddTransition("Food", "*", sOn, 50, 0);
		sOn.AddTransition("Food", "Charcoal", sOn, 50, 0);
		state = sOff;

	}

	public override bool Add(DragObject dragObject) {
		string iconName = dragObject.GetItemName();
		IconData data = Services.Instance.IconInstancer.GetData(iconName);
		if (data == null || !ChangeState(data)) {
			return false;
		}
		else {
			dragObject.Destroy();
			if (state.Name == "On") {
				sprite.Frame = onFrame;
			}
			else { 
				sprite.Frame = offFrame;
			}
			return true;
		}
	}

	private bool ChangeState(IconData data) {
		string input = "";
		if (state.IsValidInput(data.name))
			input = data.name;
		else { 
			foreach (string category in data.GetCategories()) { 
				if (state.IsValidInput(category)) { 
					input = category;
					break;
				}
			}
		}

		if (input == "") return false;
		int roll = Services.Instance.RNG.RandiRange(0, 100);
		To transition = state.GetOutput(input, roll, 0);
		string output = transition.output;
		state = transition.state;
		// Handle special cases
		if (output.Contains("[")) {
			output = output.Substr(1, output.Length - 2);
			output = output + " " + data.name;
		}
		else if (output.Contains("*")) {
			output = data.name;
		}
		DragObject dragObject = iconSpawner.Spawn(output, GlobalPosition);
		if (dragObject == null) { 
			dragObject = iconSpawner.SpawnFromCategory(output, GlobalPosition);
		}
		return true;
	}
}
