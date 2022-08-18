using Godot;
using System;
using System.Collections.Generic;

public class CampSite : MousePoint {
	[Export] int onFrame;
	[Export] int offFrame;
	IconSpawner iconSpawner = new IconSpawner();
	StateMachine stateMachine;

	// TODO
	// How do we handle partial state changes
	// OOP for state machines i.e. composite state machines
	// So we need a way to take multiple state machines
	// and compose them

	public override void _Ready() {
		base._Ready();
		sprite.Frame = offFrame;
		stateMachine = new CampSiteMachine();
	}

	public override bool Add(DragObject dragObject) {
		string iconName = dragObject.GetItemName();
		IconData data = Services.Instance.IconInstancer.GetData(iconName);
		if (data == null || !stateMachine.ValidStateChange(data)) {
			return false;
		}
		else {
			string output = stateMachine.ChangeState(data);
			DragObject outObject = iconSpawner.Spawn(output, GlobalPosition);
			if (outObject == null) { 
				outObject = iconSpawner.SpawnFromCategory(output, GlobalPosition);
			}

			dragObject.Destroy();
			State state = stateMachine.GetCurrentState();
			if (state.Name == "On") {
				sprite.Frame = onFrame;
			}
			else { 
				sprite.Frame = offFrame;
			}
			return true;
		}
	}
}
