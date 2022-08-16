using Godot;
using System;
using System.Collections.Generic;
using Caravaner;

public class WorldTiles : DropPoint {
	Dictionary<Vector2Int, int> map;
	float scale = 64f;
	Sprite selector;

	public override void _Ready() {
		map = new Dictionary<Vector2Int, int>();
		selector = (Sprite)GetNode("Selector");
	}

	private Vector2Int WorldToGrid(Vector2 pos) {
		return new Vector2Int(Mathf.RoundToInt((pos.x) / scale), - Mathf.RoundToInt((pos.y) / scale));
	}

	private Vector2 GridToWorld(Vector2Int pos) {
		return new Vector2(pos.x * scale, -pos.y * scale);
	}

	private Vector2 GetMouseTarget() {
		Vector2 pos = GetGlobalMousePosition();
		return pos;
		//return new Vector2(pos.x, pos.y + 64f);
	}

	public override void _Process(float delta) {
		base._Process(delta);
		if (IsOpen() && IsActive(null) && DragObject.IsDraggingTypeOf(1)) {
			OnMouseEntered();
		}
		else if (!IsOpen() && IsActive(this)) {
			OnMouseExited();
		}

		if (Input.IsActionJustPressed("interact")) {
			NewWorld();
		}
	}

	public void NewWorld() {
		RegionGenerator regionGenerator = new RegionGenerator();
		foreach (WorldTile worldTile in regionGenerator.Generate(-5, -5, 5, 5)) {
			CreateTile(worldTile.position, worldTile.tileName);
		}
	}

	public override bool Add(DragObject dragObject) {
		if (!IsOpen()) return false;
		Vector2Int gridPos = WorldToGrid(GetMouseTarget());
		Node2D newNode = CreateTile(gridPos, dragObject.GetItemName());
		if (newNode != null) { 
			dragObject.Destroy();
			return true;
		}
		return false;
	}

	public Node2D CreateTile(Vector2Int gridPos, string type) {
		if (map.ContainsKey(gridPos)) {
			GD.PrintErr("Cannot create a tile at that location.");
			return null;
		}
		map.Add(gridPos, 1);
		return Services.Instance.TileInstancer.Create(GridToWorld(gridPos), type);
	}

	public override bool IsOpen() {
		return !map.ContainsKey(WorldToGrid(GetMouseTarget()));
	}

	public override bool Remove(DragObject dragObject) { return false; }

	protected override void Preview(bool preview) {
		if (preview) {
			selector.GlobalPosition = GridToWorld(WorldToGrid(GetMouseTarget()));
		}
		else {
			selector.GlobalPosition = new Vector2(100000, 100000);
		}
	}
}
