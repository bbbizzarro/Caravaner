using System;
using System.Collections.Generic;
using Godot;
public class StateMachine {

	protected State currentState;
	private Dictionary<string, State> states;

	public StateMachine() {
		states = new Dictionary<string, State>();
	}

	public StateMachine(IEnumerable<TransitionData> data) { 
		states = new Dictionary<string, State>();
		foreach (TransitionData t in data) { 
			if (!states.ContainsKey(t.fromState)) {
				states.Add(t.fromState, new State(t.fromState));
			}
			if (!states.ContainsKey(t.toState)) {
				states.Add(t.toState, new State(t.toState));
			}
			states[t.fromState]
				.AddTransition(t.input, t.output, states[t.toState], t.probability, t.inputFactor);
		}
	}

	public void SetCurrentState(string stateName) {
		if (!states.ContainsKey(stateName)) {
			GD.PrintErr(String.Format("State Machine has no state called {0}", stateName));
			return;
		}
		currentState = states[stateName];
	}

	// We might not need this! We can just have a list of state 
	// machines and do it that way! Have each state machine
	// be independent
	public void Compose(StateMachine stateMachine) { 
		if (states == null) {
			states = new Dictionary<string, State>();
		}
		foreach (State state in stateMachine.GetStates()) { 
			
		}
	}

	protected IEnumerable<State> GetStates() {
		return states.Values;
	}

	public State GetCurrentState() {
		return currentState;
	}

	public bool ValidStateChange(IconData data) { 
		string input = "";
		if (currentState.IsValidInput(data.name))
			input = data.name;
		else { 
			foreach (string category in data.GetCategories()) { 
				if (currentState.IsValidInput(category)) { 
					input = category;
					break;
				}
			}
		}

		if (input == "") return false;
		return true;
	}

	public string ChangeState(IconData data) {
		string input = "";
		if (currentState.IsValidInput(data.name))
			input = data.name;
		else { 
			foreach (string category in data.GetCategories()) { 
				if (currentState.IsValidInput(category)) { 
					input = category;
					break;
				}
			}
		}

		if (input == "") return "NULL";
		int roll = Services.Instance.RNG.RandiRange(0, 100);
		To transition = currentState.GetOutput(input, roll, 0);
		string output = transition.output;
		currentState = transition.state;
		// Handle special cases
		if (output.Contains("[")) {
			output = output.Substr(1, output.Length - 2);
			output = output + " " + data.name;
		}
		else if (output.Contains("*")) {
			output = data.name;
		}
		return output;
	}
}

public class CampSiteMachine : StateMachine { 
	public CampSiteMachine() { 
		var sOn = new State("On");
		var sOff = new State("Off");

		sOff.AddTransition("Paper", "Charcoal", sOn, 50, 0);
		sOff.AddTransition("Paper", "", sOn, 50, 0);

		sOn.AddTransition("Raw Food", "[Cooked]", sOn, 90, 0);
		sOn.AddTransition("Raw Food", "Charcoal", sOn, 10, 0);
		sOn.AddTransition("Paper", "Charcoal", sOn, 100, 0);
		sOn.AddTransition("Food", "*", sOn, 50, 0);
		sOn.AddTransition("Food", "Charcoal", sOn, 50, 0);
		currentState = sOff;
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

public class To {
	public State state;
	public string output;
	public int bin;
	public int probability;
	public int inputFactor;
	public To(State state, string output, int bin, int probability, int inputFactor) {
		this.state = state;
		this.output = output;
		this.bin = bin;
		this.probability = probability;
		this.inputFactor = inputFactor;
	}
}

public class State {
	public string Name { get; private set; }
	private readonly Dictionary<string, List<To>> states
		= new Dictionary<string, List<To>>();
	
	public State(string name) {
		Name = name;
	}

	public State AddTransition(string input, string output, State state, int probability, int inputFactor) {
		if (!states.ContainsKey(input))
			states.Add(input, new List<To>());
		states[input].Add(new To(state, output, 0, probability, inputFactor));
		// Sum probabilities 
		int total = 0;
		foreach (To t in states[input]) {
			total += t.probability;
		}
		// Partition range
		int curr = 0;
		foreach (To t in states[input]) {
			t.bin = curr + Mathf.RoundToInt(100f * (float)t.probability / (float)total);
			curr = t.bin;
			//GD.Print(String.Format("OUTPUT: {0} ||| BIN: {1}", t.output, t.bin));
		}
		return state;
	}

	public bool IsValidInput(string input) {
		return states.ContainsKey(input);
	}

	public To GetOutput(string input, int roll, int rollModifier) {
		if (!IsValidInput(input)) {
			return null ;
		}
		foreach (To possibility in states[input]) {
			GD.Print("ROLL: ", roll, " BIN: ", possibility.bin + possibility.inputFactor * rollModifier);
			if (roll <= possibility.bin + possibility.inputFactor * rollModifier) {
				return possibility;
			}
		}
		return null;
	}
}
