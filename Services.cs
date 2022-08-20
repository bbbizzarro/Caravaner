using Godot;
using System;

public class Services : Node {
	public static Services Instance { get; private set; }

	public SpriteDB SpriteDB { get; private set; }
	public IconInstancer IconInstancer { get; private set; }
	public TileInstancer TileInstancer { get; private set; }
	public RandomNumberGenerator RNG { get; private set; }
	public StateMachineDB StateMachineDB { get; private set; }


	public override void _Ready() {
		if (Instance != null) QueueFree();
		else Instance = this;

		SpriteDB = new SpriteDB();
		IconInstancer = new IconInstancer(GetParent());
		TileInstancer = new TileInstancer(GetParent());
		RNG = new RandomNumberGenerator();
		RNG.Randomize();
		StateMachineDB = new StateMachineDB("res://caravaner_states_db.json");
	}
}
