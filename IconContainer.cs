using Godot;
using System;
using System.Collections.Generic;

public class IconContainer : Node2D {
	static IconContainer currIconContainer;
	DragDropHandler dragDropHandler;
	List<DragObject> icons;
	int maxContainerSize = 1;

	public static bool DropIn(DragObject dragObject) { 
		if (currIconContainer != null && currIconContainer.IsOpen()) {
			dragObject.SetPosition(currIconContainer.Position);
			dragObject.SetIconContainer(currIconContainer);
			currIconContainer.AddIcon(dragObject);
			return true;
		}
		else {
			return false;
		}
	}

	public bool IsOpen() {
		return icons.Count < maxContainerSize;
	}

	public void AddIcon(DragObject icon) { 
		if (IsOpen()) {
			icons.Add(icon);
		}
	}

	public void RemoveIcon(DragObject icon) { 
		if (icons.Contains(icon)) {
			icons.Remove(icon);
		}
	}

	public override void _Ready() {
		icons = new List<DragObject>();
		dragDropHandler = (DragDropHandler)GetNode("/root/Main/DragDropHandler");
		Connect("mouse_entered", this, nameof(OnMouseEntered));
		Connect("mouse_exited", this, nameof(OnMouseExited));
	}

	private void OnMouseEntered() { 
		if (currIconContainer == null) {
			IconContainer.currIconContainer = this;
			GD.Print("Mouse over container.");
		}
	}

	private void OnMouseExited() { 
		if (currIconContainer == this) {
			IconContainer.currIconContainer = null;
			GD.Print("Mouse exit container.");
		}
	}
}
