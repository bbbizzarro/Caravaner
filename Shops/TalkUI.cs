using Godot;
using System;

public class TalkUI : Node2D {
    IGButton buy;
    IGButton sell;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        buy = (IGButton)GetNode("Buy");
        sell = (IGButton)GetNode("Sell");
    }

    public void Initialize(IGButtonEvent buyCall, IGButtonEvent sellCall) {
        buy.ButtonClickedEvent += buyCall;
    }
}
