using Godot;
using System;

public class CityCenter : Node2D, RegionObject {
    private Label ImportLabel;

    public override void _Ready() {
    }

    public void SetLabel(string importLabel) {
        ImportLabel = (Label)GetNode("ImportLabel");
        ImportLabel.Text = importLabel;
    }

    public void Initialize(Region region) {
        SetLabel(region.Import);
    }
}


public interface RegionObject {
    void Initialize(Region region);
}