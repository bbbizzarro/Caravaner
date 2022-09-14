using Godot;
using System.Collections.Generic;

public class ButtonMenu {

    private List<IGButton> buttons;
    private int buttonSize;
    private int spacing;

    public ButtonMenu(int buttonSize, int spacing) {
        buttons = new List<IGButton>();
        this.buttonSize = buttonSize;
        this.spacing = spacing;
    }

    public void Add(IGButton button) {
        buttons.Add(button);
        button.ButtonDestroyedEvent += Remove;
        ArrangeButtons();
    }

    public void Clear() {
        List<IGButton> b = new List<IGButton>(buttons);
        foreach (var i in b) {
            i.Destroy();
        }
    }

    public void Remove(IGButton button) {
        buttons.Remove(button);
        ArrangeButtons();
    }

    protected void ArrangeButtons() {
        float rowOffset = buttonSize * (buttons.Count - 1f) / 2f;
        for (int i = 0; i < buttons.Count; ++i) {
            buttons[i].Position = new Vector2(i * buttonSize - rowOffset, 0);
        }
    }
}