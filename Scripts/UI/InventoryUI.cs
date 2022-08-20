using Godot;
using System;
using System.Collections.Generic;

public class InventoryUI : DropPoint {
	List<DragObject> icons;
	[Export] int maxContainerSize = 5;
	[Export] int scale = 96;

	// Godot Engine functions ===============================
	public override void _Process(float delta) {
		base._Process(delta);
	}

	public override void _Ready() {
		base._Ready();
		icons = new List<DragObject>();
	}


	private int MouseToIndex(int offset) {
		// Get the x coordinate relative to the icon position.
		float x = GetGlobalMousePosition().x - GlobalPosition.x;
		x = (x/scale) + (float)(Mathf.Max(0, icons.Count - 1 + offset)) / 2f;
		return Mathf.Clamp(Mathf.RoundToInt(x), 0, icons.Count + 1);
	}

	private void RepositionSlots(int count, int skip) {
		float amt = scale / 2 * (count - 1);
		List<Vector2> newPositions = new List<Vector2>();
		if (count == 1) {
			newPositions.Add(Vector2.Zero);
		}
		else { 
			for (float p = -amt; p <= amt; p += scale) {
				newPositions.Add(new Vector2(p, 0));
			}
		}
		int offset = 0;
		for (int i = 0; i < newPositions.Count; ++i) {
			if (i == skip) {
				offset = 1;
			}
			else { 
				if (i - offset < 0 || i - offset >= icons.Count || i < 0 || i >= newPositions.Count){
					
				}
				else if ((icons[i - offset].Position - newPositions[i]).Length() <= 0.01f) {
					// do nothing
				}
				else { 
					icons[i - offset].AnimateToPosition(icons[i - offset].Position, newPositions[i]);
				}
			}
		}
	}

	protected override void Preview(bool preview) {
		if (preview) { 
			if (DragObject.IsDragging() && IsOpen()) {
				RepositionSlots(icons.Count + 1, MouseToIndex(1));
			}
		}
		else {
			RepositionSlots(icons.Count, -1);
		}
	}


	public override bool IsOpen() {
		return icons.Count < maxContainerSize;
	}

	public override bool Add(DragObject dragObject) {
		if (IsOpen() && !icons.Contains(dragObject)) {
			icons.Insert(MouseToIndex(1), dragObject);
			dragObject.SetIconContainer(this);
			Vector2 relPos = dragObject.Position - GlobalPosition;
			dragObject.GetParent().RemoveChild(dragObject);
			AddChild(dragObject);
			dragObject.Position = relPos;
			RepositionSlots(icons.Count, -1);
			collShape.Scale = new Vector2(icons.Count + 1.5f, collShape.Scale.y);
			return true;
		}
		return false;
	}

	public override bool Remove(DragObject dragObject) {
		if (icons.Contains(dragObject)) {
			Vector2 relPos = GlobalPosition + dragObject.Position;
			dragObject.Position = relPos;
			icons.Remove(dragObject);
			dragObject.GetParent().RemoveChild(dragObject);
			Services.Instance.Main.AddChild(dragObject);
			RepositionSlots(icons.Count, -1);
			collShape.Scale = new Vector2(icons.Count + 1.5f, collShape.Scale.y);
			return true;
		}
		return false;
	}

	protected override void OnSet() {
		Preview(true);
	}

	protected override void OnRelease() {
		Preview(false);
	}

}
