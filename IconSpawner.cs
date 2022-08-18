using Godot;
using System;

public class IconSpawner {
	RandomNumberGenerator rng = new RandomNumberGenerator();

	public IconSpawner() {
		rng.Randomize();
	}

	public void SpawnGroup(int count, Vector2 centerPosition) {
		if (count == 0) return;
		else if (count == 1) {
			Spawn(rng.RandfRange(-0.5f, 0.5f), centerPosition);
			return;
		}
		else if (count == 2) { 
			Spawn(rng.RandfRange(-1f, -0.2f), centerPosition);
			Spawn(rng.RandfRange(0.2f, 1f), centerPosition);
			return;
		}

		float delta = 2f / (count - 1f);
		for (float i = -1; i <= 1; i += delta) {
			Spawn(i, centerPosition);
		}
	}

	public DragObject SpawnFromCategory(string category, Vector2 centerPosition) {
		DragObject dragObject = Services.Instance.IconInstancer.CreateFromCategory(centerPosition, category);
		if (dragObject == null) return null;
		float speed = 200f;
		Vector2 velocity = new Vector2(0, -rng.RandfRange(0.2f, 1f));
		velocity = velocity.Normalized();
		dragObject.Initialize(centerPosition, speed * velocity, rng.RandfRange(0, 128f));
		return dragObject;
	}

	public DragObject Spawn(string name, Vector2 centerPosition) {
		DragObject dragObject = Services.Instance.IconInstancer.Create(centerPosition, name);
		if (dragObject == null) return null;
		float speed = 200f;
		Vector2 velocity = new Vector2(0, -rng.RandfRange(0.2f, 1f));
		velocity = velocity.Normalized();
		dragObject.Initialize(centerPosition, speed * velocity, rng.RandfRange(0, 128f));
		return dragObject;
	}

	public void Spawn(float x, Vector2 centerPosition) {
		DragObject dragObject = Services.Instance.IconInstancer.CreateRandom(centerPosition);
		if (dragObject == null) return;
		float speed = 200f;
		Vector2 velocity = new Vector2(x, -rng.RandfRange(0.2f, 1f));
		velocity = velocity.Normalized();
		dragObject.Initialize(centerPosition, speed * velocity, rng.RandfRange(0, 128f));
	}

}
