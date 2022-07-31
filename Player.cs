using Godot;
using Godot.Collections;
using System;
using Caravaner;

public class Player : KinematicBody2D, ISavable {

	[SerializeField] private float speed = 100f;
	[SerializeField] private int scale = 64;

	private Vector2 direction = Vector2.Zero;
	private Vector2Int lastPos = Vector2Int.Zero;
	//private readonly float MAX_FRAME_RATE = 60f;
	[Signal] delegate void GridPositionChanged();
	World world;

	public void Initialize(World world) {
		this.world = world;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta) {
		direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		//MoveAndCollide(MAX_FRAME_RATE * delta * speed * direction);
		MoveAndSlide(speed * direction);
		TrackLocation();
		if (Input.IsActionJustPressed("interact")) {
			Interact();
		}
	}

	private void Interact() {
		Tile tile = world.GetTile(Position);
		if (tile != null) {
			GD.Print(tile.type);
		}
	}

	private void TrackLocation() {
		Vector2Int pos = new Vector2Int(Mathf.RoundToInt(Position.x / scale), 
										Mathf.RoundToInt(Position.y / scale));
		if (pos != lastPos) {
			lastPos = pos;
			EmitSignal(nameof(GridPositionChanged));
		}
	}

	public Vector2Int GetGridPosition() {
		return lastPos;
	}

	public Godot.Collections.Dictionary<string, object> Save() {
		var data = JSONUtils.SerializeNode(this);
		data["PosX"] = Position.x;
		data["PosY"] = Position.y;
		return data;
	}

	public void Load(Dictionary<string, object> data) {
		if (data.ContainsKey("PosX") && data.ContainsKey("PosY")) {
			Position = new Vector2((float)data["PosX"], (float)data["PosY"]);
		}
	}
}
