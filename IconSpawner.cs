using Godot;
using System;

public class IconSpawner : Node2D {
	PackedScene iconScene = (PackedScene)ResourceLoader.Load("res://DragObject.tscn");
	RandomNumberGenerator rng = new RandomNumberGenerator();

	public override void _Ready() {
		rng.Randomize();
	}

	public override void _Process(float delta) {
		if (Input.IsActionJustPressed("interact")) {
			SpawnGroup(3);
		}
	}
	private void SpawnGroup(int count) {
		if (count == 0) return;
		else if (count == 1) {
			Spawn(rng.RandfRange(-0.5f, 0.5f));
			return;
		}
		else if (count == 2) { 
			Spawn(rng.RandfRange(-1f, -0.2f));
			Spawn(rng.RandfRange(0.2f, 1f));
			return;
		}

		float delta = 2f / (count - 1f);
		for (float i = -1; i <= 1; i += delta) {
			Spawn(i);
		}
	}
	private void Spawn(float x) { 
		DragObject dragObject = (DragObject)iconScene.Instance();
		GetParent().AddChild(dragObject);
		float speed = 200f;
		Vector2 velocity = new Vector2(x, -rng.RandfRange(0.2f, 1f));
		velocity = velocity.Normalized();
		dragObject.Initialize(Position, speed * velocity, rng.RandfRange(0, 128f));
	}

}
