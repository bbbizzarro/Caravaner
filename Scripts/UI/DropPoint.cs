using Godot;
using System;
using System.Collections.Generic;

public abstract class DropPoint : Node2D {
	static DropPoint currDropPoint;
	protected bool mouseIsOver;
	bool enabled = true;
	protected CollisionShape2D collShape;

	protected static void SetCurrDropPoint(DropPoint iconContainer) {
		if (currDropPoint != null && currDropPoint != iconContainer) {
			currDropPoint.OnMouseExited();
		}
		currDropPoint = iconContainer;
		currDropPoint.OnSet();
	}

	protected static void ReleaseCurrDropPoint(DropPoint iconContainer) {
		if (currDropPoint == iconContainer) { 
			currDropPoint.OnRelease();
			currDropPoint = null;
		}
	}

	protected static bool IsActive(DropPoint iconContainer) {
		return currDropPoint == iconContainer;
	}

	// Godot Engine functions ===============================
	public override void _Process(float delta) {
		OnMouseStay();
	}

	public override void _Ready() {
		collShape = (CollisionShape2D)GetNode("CollisionShape2D");
		Connect("mouse_entered", this, nameof(OnMouseEntered));
		Connect("mouse_exited", this, nameof(OnMouseExited));
	}

	// Abstract Interface ===================================
	public abstract bool IsOpen();
	public abstract bool Add(DragObject dragObject);
	public abstract bool Remove(DragObject dragObject);
	protected abstract void Preview(bool preview);
	protected virtual void OnSet() {}
	protected virtual void OnRelease() {}

	// Handle Mouse Interaction =============================
	public static bool DropIn(DragObject dragObject) { 
		if (currDropPoint != null && currDropPoint.IsOpen()) {
			return currDropPoint.Add(dragObject);
		}
		else return false;
	}

	private void OnMouseStay() { 
		if (mouseIsOver) {
			SetCurrDropPoint(this);
			currDropPoint.Preview(true);
		}
	}

	protected void OnMouseEntered() {
		mouseIsOver = true;
		SetCurrDropPoint(this);
	}

	protected void OnMouseExited() { 
		if (IsActive(this)) {
			mouseIsOver = false;
			currDropPoint.Preview(false);
			ReleaseCurrDropPoint(this);
		}
	}

	// Entity Enable/Disable ================================
	public bool IsEnabled() {
		return enabled;
	}

	public void Enable(bool enable) { 
		if (enable) {
			enabled = true;
			Position = new Vector2(0, 0);
		}
		else {
			GlobalPosition = new Vector2(-1000, 1000);
			enabled = false;
		}
	}
}
