using Godot;
using System;

public class PlayerDropPoint : DropPoint {

	public override bool Add(DragObject dragObject) {
		IconData iconData = Services.Instance.IconInstancer.GetData(dragObject.GetItemName());
		if (iconData.InCategory("Food")) { 
			dragObject.Destroy();
			return true;
		}
		return false;
	}

	protected override void Preview(bool preview) {
		if (preview) { 
			DragObject.SetMouseOffset(new Vector2(0, -64f));
		}
		else { 
			DragObject.SetMouseOffset(Vector2.Zero);
		}
	}
}
