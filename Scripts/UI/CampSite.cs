using Godot;
using System;
using System.Collections.Generic;

public class CampSite : MousePoint {
	[Export] int onFrame;
	[Export] int offFrame;
	State state;
	IconSpawner iconSpawner = new IconSpawner();

	public override void _Ready() {
		base._Ready();
		sprite.Frame = offFrame;
		state = new State();
		state.AddTransition("Paper")
			 .AddTransition("Raw Food", "Green Mulk", state);

	}

	public override bool Add(DragObject dragObject) {
		string iconName = dragObject.GetItemName();
		IconData data = Services.Instance.IconInstancer.GetData(iconName);
		if (data == null || !ChangeState(data)) {
			return false;
		}
		else {
			dragObject.Destroy();
			return true;
		}
	}

	private bool ChangeState(IconData data) {
		string input;
		if (state.ValidInput(data.majorCategory))
			input = data.majorCategory;
		else if (state.ValidInput(data.minorCategory))
			input = data.minorCategory;
		else return false;
		string output = state.Output(input);
		GD.Print("New state!");
		state = state.Next(input);
		iconSpawner.Spawn(output, GlobalPosition);
		return true;
	}

	private void HandleOutput(string output) {
	}

}

// For more complex state changes we can have conditionals
// that check surrounding tiles.
// Need to create a UI to be able to create these state machines
// easily.

// Should also add the potential for random states transitions
// Could also add an editor scene where we drag and drop state 
// nodes (.tscn).
// Push a button and it generates code that you can copy and paste
// into the _Ready() function of another script or we can just
// load and ref those scenes directly when we _Ready() a new object
// At the moment, though we need to just explore what we might want
// Before we create tools to help us speed up the process.

public class State {
	private readonly Dictionary<string, (string, State)> states;
	
	public State() {
		states = new Dictionary<string, (string, State)>();
	}

	public State AddTransition(string input) {
		State state = new State();
		states.Add(input, ("", state));
		return state;
	}

	public State AddTransition(string input, string output) {
		State state = new State();
		states.Add(input, (output, state));
		return state;
	}

	public State AddTransition(string input, string output, State state) {
		states.Add(input, (output, state));
		return state;
	}

	public bool ValidInput(string input) {
		return states.ContainsKey(input);
	}

	public string Output(string input) { 
		if (states.ContainsKey(input)) {
			return states[input].Item1;
		}
		else {
			return "";
		}
	}

	public State Next(string input) {
		if (states.ContainsKey(input)) {
			return states[input].Item2;
		}
		return null;
	}
}
