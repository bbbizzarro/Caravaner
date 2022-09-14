using Godot;

public delegate void IGButtonEvent(IGButton button);

public class IGButton : Area2D {

    static IGButton mouseOverButton;
    public event IGButtonEvent ButtonDestroyedEvent;
    public event IGButtonEvent ButtonClickedEvent;

    public override void _Ready() {
        base._Ready();
        Connect("mouse_entered", this, nameof(HandleMouseEnterEvent));
        Connect("mouse_exited", this, nameof(HandleMouseExitEvent));
    }

    public override void _Process(float delta) {
        base._Process(delta);
        if (MouseIsOverButton() && ButtonWasClicked()) {
            OnClick();
        }
    }

    public void Destroy() {
        ButtonDestroyedEvent?.Invoke(this);
        QueueFree();
    }

    public bool ButtonWasClicked() {
        return Input.IsActionJustPressed("ui_click");
    }

    protected bool MouseIsOverButton() {
        return mouseOverButton == this;
    }

    protected void HandleMouseEnterEvent() {
        if (mouseOverButton != null) {
            mouseOverButton.HandleMouseExitEvent();
        }
        mouseOverButton = this;
        OnMouseEntered();
    }
    protected void HandleMouseExitEvent() {
        if (MouseIsOverButton()) {
            mouseOverButton = null;
            OnMouseExited();
        }
    }

    protected virtual void OnMouseEntered() {
    }
    protected virtual void OnMouseExited() {
    }
    protected virtual void OnClick() {
        ButtonClickedEvent?.Invoke(this);
    }
}