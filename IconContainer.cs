using Godot;
using System;
using System.Collections.Generic;

public class IconContainer : Node2D {
	static IconContainer currIconContainer;
	DragDropHandler dragDropHandler;
	List<DragObject> icons;
	//DragObject[] icons;
	Vector2[] slotPositions;
	int maxContainerSize = 5;
	CollisionShape2D collShape;
	int scale = 96;

	public static bool DropIn(DragObject dragObject) { 
		if (currIconContainer != null && currIconContainer.IsOpen()) {
			currIconContainer.AddIcon(dragObject);
			return true;
		}
		else {
			return false;
		}
	}

	//private int NumIconsInContainer() {
	//	int total = 0;
	//	for (int i = 0; i < maxContainerSize; ++i) { 
	//		if (icons[i] != null) {
	//			total += 1;
	//		}
	//	}
	//	return total;
	//}

	public bool IsOpen() {
		return icons.Count < maxContainerSize;
	}

	public int MouseToIndex(int offset) {
		float x = GetGlobalMousePosition().x - Position.x;
		x = (x/scale) + (float)(Mathf.Max(0, icons.Count -1 + offset)) / 2f;
		//GD.Print("X=", x);
		int index = Mathf.Clamp(Mathf.RoundToInt(x), 0, icons.Count + 1);
		GD.Print("INDEX=", index);
		return index;
	}

	public void AddIcon(DragObject icon) { 
		if (IsOpen() && !icons.Contains(icon)) {
			//icons.Add(icon);
			icons.Insert(MouseToIndex(1), icon);
			icon.SetIconContainer(currIconContainer);
			Vector2 relPos = icon.Position - Position;
			icon.GetParent().RemoveChild(icon);
			currIconContainer.AddChild(icon);
			icon.Position = relPos;
			RepositionSlots(icons.Count, -1);
			//icon.AnimateToPosition(relPos, Vector2.Zero);
			collShape.Scale = new Vector2(icons.Count + 1, collShape.Scale.y);
		}
	}

	public void RemoveIcon(DragObject icon) { 
		if (icons.Contains(icon)) {
			Vector2 relPos = Position + icon.Position;
			icon.Position = relPos;
			icons.Remove(icon);
			icon.GetParent().RemoveChild(icon);
			GetNode("/root/Main").AddChild(icon);
			RepositionSlots(icons.Count, -1);
			collShape.Scale = new Vector2(icons.Count + 1, collShape.Scale.y);
		}
	}

	private void RepositionSlots(int count, int skip) {
		// 0 -> -32 32 -> -64 0 64 ->  
		// 0 -> 128 / n * n/2
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
				icons[i - offset].AnimateToPosition(icons[i - offset].Position, newPositions[i]);
			}
		}
	}

	public override void _Ready() {
		icons = new List<DragObject>();
		dragDropHandler = (DragDropHandler)GetNode("/root/Main/DragDropHandler");
		slotPositions = new Vector2[maxContainerSize];
		collShape = (CollisionShape2D)GetNode("CollisionShape2D");
		Connect("mouse_entered", this, nameof(OnMouseEntered));
		Connect("mouse_exited", this, nameof(OnMouseExited));
	}

	public void Preview(bool preview) { 
		if (preview) { 
			if (DragObject.IsDragging()) {
				RepositionSlots(icons.Count + 1, MouseToIndex(1));
			}
		}
		else {
			RepositionSlots(icons.Count, -1);
		}
	}

	private void OnMouseEntered() { 
		if (currIconContainer == null) {
			IconContainer.currIconContainer = this;
			currIconContainer.Preview(true);
			GD.Print(currIconContainer.MouseToIndex(0));
		}
	}

	private void OnMouseExited() { 
		if (currIconContainer == this) {
			currIconContainer.Preview(false);	
			IconContainer.currIconContainer = null;
		}
	}
}
