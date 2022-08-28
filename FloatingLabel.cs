using Godot;
using System;

public class FloatingLabel : Node2D {

    Label label;

    public override void _Ready() {
        if (label == null) {
            label = (Label)GetNode("Label");     
        }
    }

    public void SetLabel(string text) {
        if (label == null) {
            label = (Label)GetNode("Label");     
        }
        label.Text = text;
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
