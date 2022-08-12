using Godot;
using System;

public class IconSpawner {
	PackedScene iconScene = (PackedScene)ResourceLoader.Load("res://DragObject.tscn");
	RandomNumberGenerator rng = new RandomNumberGenerator();

	public IconSpawner() {
		rng.Randomize();
	}

	public void SpawnGroup(Node parent, int count, Vector2 centerPosition) {
		if (count == 0) return;
		else if (count == 1) {
			Spawn(parent, rng.RandfRange(-0.5f, 0.5f), centerPosition);
			return;
		}
		else if (count == 2) { 
			Spawn(parent, rng.RandfRange(-1f, -0.2f), centerPosition);
			Spawn(parent, rng.RandfRange(0.2f, 1f), centerPosition);
			return;
		}

		float delta = 2f / (count - 1f);
		for (float i = -1; i <= 1; i += delta) {
			Spawn(parent, i, centerPosition);
		}
	}
	public void Spawn(Node parent, float x, Vector2 centerPosition) { 
		DragObject dragObject = (DragObject)iconScene.Instance();
		parent.AddChild(dragObject);
		float speed = 200f;
		Vector2 velocity = new Vector2(x, -rng.RandfRange(0.2f, 1f));
		velocity = velocity.Normalized();
		dragObject.Initialize(centerPosition, speed * velocity, rng.RandfRange(0, 128f));
	}

}
