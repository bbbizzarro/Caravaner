using Godot;
using System;
using System.Collections.Generic;

public class Shop : Node2D {
    [Export] public int ButtonCount;
    [Export] public string ShopButtonScenePath;

	PackedScene shopButtonScene; 


    public Dictionary<string, ItemStack> items; 
    ShopBuyUI shopBuyState;
    TalkUI talkUI;
    Node currentState;
    Node toRemove;

    public override void _Ready() {
        shopButtonScene = (PackedScene)ResourceLoader.Load(ShopButtonScenePath);
        var igButtonScene = (PackedScene)ResourceLoader.Load("res://Shops/IGButton.tscn");
        items = new Dictionary<string, ItemStack>() {
            {"test", new ItemStack() {name = "test", amount = 2, cost = 1}},
            {"test1", new ItemStack() {name = "test1", amount = 4, cost = 1}},
            {"test2", new ItemStack() {name = "test2", amount = 1, cost = 1}},
            {"test3", new ItemStack() {name = "test3", amount = 6, cost = 1}},
            {"test4", new ItemStack() {name = "test4", amount = 1, cost = 1}}
        };
        shopBuyState = (ShopBuyUI)GetNode("ShopBuyUI");
        shopBuyState.Initialize(this, shopButtonScene);
        shopBuyState.OnEnter();
        RemoveChild(shopBuyState);
        talkUI = (TalkUI)GetNode("TalkUI");
        talkUI.Initialize(SwitchToShopBuyState, null);
        currentState = talkUI;
    }

    public void SwitchToShopBuyState(IGButton button) {
        if (currentState != shopBuyState) {
            AddChild(shopBuyState);
            RemoveChild(currentState);
            currentState = shopBuyState;
        }
    }

    public override void _Process(float delta) {
        base._Process(delta);
        if (Input.IsActionJustPressed("interact")) {
        }
    }
}

public interface State {
}

public class ItemStack{
    public string name;
    public int amount;
    public int cost;
}
    

public interface IItemStack {
    string GetName();
    int GetAmount();
    int GetSingleItemCost();
    void UpdateAmount(int amount);
}

public class DebugPlayer : IBuyer {
    public int amount;

    public DebugPlayer(int amount) {
        this.amount = amount;
    }

    public int GetAmount() {
        return amount;
    }
}

public interface IBuyer {
    int GetAmount();
}