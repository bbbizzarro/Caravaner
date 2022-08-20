using Godot;
using System;
using System.Collections.Generic;

public delegate void HandleEvent();

public class CampSite : MousePoint {
	[Export] int onFrame;
	[Export] int offFrame;
	[Export] float cookingTime = 2f;
	IconSpawner iconSpawner = new IconSpawner();
	Timer timer;
	event HandleEvent timerCompleteEvent;

	StateBase currentState;

	// States
	OnState onState;
	OffState offState;
	CookingState cookingState;

	public override void _Ready() {
		base._Ready();
		timer = (Timer)GetNode("Timer");
		onState = new OnState(this);
		offState = new OffState(this);
		cookingState = new CookingState(this);
		ChangeState(offState);
		timer.Connect("timeout", this, nameof(TimerComplete));
	}

	public override bool Add(DragObject dragObject) {
		IconData input = Services.Instance.IconInstancer.GetData(dragObject.GetItemName());
		if (currentState.Execute(input)) {
			dragObject.Destroy();
			return true;
		}
		return false;
	}

	private void ChangeState(StateBase state) {
		StateBase nextState = state;
		if (currentState != null)
			currentState.OnExit();
		currentState = nextState;
		if (nextState != null)
			nextState.OnEnter();
	}

	private void SetSprite(int frame) {
		sprite.Frame = frame;
	}

	private void Spawn(string output) {
		iconSpawner.Spawn(output, GlobalPosition);
	}

	private void TimerComplete() {
		timerCompleteEvent?.Invoke();
	}

	private class CookingState : StateBase {
		CampSite campSite;
		public CookingState(CampSite campSite) { 
			this.campSite = campSite;
		}
		public override void OnEnter() {
			campSite.timer.Start(campSite.cookingTime);
			campSite.timerCompleteEvent += CompleteCooking;
		}

		public override void OnExit() {
			campSite.timerCompleteEvent -= CompleteCooking;
		}

		private void CompleteCooking() { 
			int roll = Services.Instance.RNG.RandiRange(0, 100);
			if (roll < 90)
				campSite.iconSpawner.SpawnFromCategory("Cooked", campSite.GlobalPosition);
			else
				campSite.Spawn("Ash");
			campSite.ChangeState(campSite.onState);
		}


		public override bool Execute(IconData input) {
			return false;
		}
	}

	private class OnState : StateBase {
		CampSite campSite;
		public OnState(CampSite campSite) { this.campSite = campSite; }

		public override void OnEnter() {
			campSite.SetSprite(campSite.onFrame);
		}

		public override bool Execute(IconData input) {
			if (input.InCategory("Raw Food")) {
				campSite.ChangeState(campSite.cookingState);
				return true;
			}
			else if (input.InCategory("Organic")) { 
				campSite.Spawn("Ash");
				return true;
			}
			else
				return false;
		}
	}

	private class OffState : StateBase {
		CampSite campSite;
		public OffState(CampSite campSite) { this.campSite = campSite; }

		public override void OnEnter() {
			campSite.SetSprite(campSite.offFrame);
		}

		public override bool Execute(IconData input) {
			if (!input.InCategory("Paper")) return false;
			campSite.ChangeState(campSite.onState);
			return true;
		}
	}
}


abstract class StateBase {
	public virtual void OnEnter() { }
	public virtual void OnExit() { }
	public abstract bool Execute(IconData input);
}

abstract class StateMachineBase {
	protected StateBase currentState;
	public virtual void ChangeState(StateBase nextState) {
		currentState = nextState;
	}
}
