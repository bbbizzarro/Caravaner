using Godot;
using System;

public delegate void ShopButtonEvent(ShopButton button);

public class ShopButton : IGButton {
    [Export] public string itemName;
    [Export] public int amount;
    [Export] public int cost;
    [Export] private NodePath amountTextPath;
    [Export] private NodePath costTextPath;

    public event ShopButtonEvent OnClickEvent;

    Label amountText;
    Label costText;

    public string name;

    public override void _Ready() {
        base._Ready();
        if (!amountTextPath.IsEmpty()) {
            amountText = (Label)GetNode(amountTextPath);
        }
        if (!costTextPath.IsEmpty()) {
            costText = (Label)GetNode(costTextPath);
        }
        UpdateButton();
    }

    public void Initialize(string name, int cost, int amount) {
        this.name = name;
        this.cost = cost;
        this.amount = amount;
    }

    public void UpdateAmount(int amount) {
        this.amount = amount;
        UpdateButton();
    }

    public void UpdateButton() {
        amountText.Text = amount.ToString();
        costText.Text = cost.ToString();
    }

    protected override void OnClick() {
        OnClickEvent?.Invoke(this);
    }
}
