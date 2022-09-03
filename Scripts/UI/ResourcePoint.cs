using System;
using Godot;
using System.Collections.Generic;

public class ResourcePoint : DropPoint {
	[Export] private string Location = "Wastes";
	private AnimationPlayer animationPlayer;
	private int integrity = 3;
	RandomNumberGenerator rng = new RandomNumberGenerator();
	private const float TimeEffort = 2f;
	private const float EnergyEffort = 3f;
	private PlayerData playerData;

	public override void _Ready() {
		base._Ready();
		animationPlayer = (AnimationPlayer)GetNode("AnimationPlayer");
		rng.Randomize();
		playerData = Services.Instance.PlayerData;
	}

	private bool AttemptToGather() {
		if (playerData.Energy >= EnergyEffort) {
			playerData.Energy -= EnergyEffort;
			return true;
		}
		return false;
	}

	protected override void OnMousePress() {
		animationPlayer.Play("StrongSquish");

		if (!AttemptToGather()) return;

		// Integrity based "mining'
		integrity -= rng.RandiRange(1, integrity);
		Services.Instance.WorldState.AdvanceTimeByTicks(TimeEffort);
		if (integrity <= 0 ) {
			List<IconData> icons = Services.Instance.IconInstancer
				.SelectMany(1, "*", "*", Location, Rarity.Common, -1);
			Services.Instance.IconInstancer
				.SpawnGroup(GlobalPosition, icons);
			Destroy();
		}
	}

	protected override void Preview(bool preview) {
		if (preview) { 
			//animationPlayer.Play("Squish");
			animationPlayer.Play("Squish");
			//DragObject.SetMouseOffset(new Vector2(0, -64f));
		}
		else { 
			animationPlayer.Play("StaticIdle");
			//DragObject.SetMouseOffset(Vector2.Zero);
		}
	}
}
