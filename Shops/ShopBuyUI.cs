using Godot;
using System.Collections.Generic;

public class ShopBuyUI : Node2D{
    Shop shop;
    PackedScene shopButtonScene;
    ButtonMenu buttonMenu;
    IGButton exitButton;

    public override void _Ready() {
        base._Ready();
    }

    public void Initialize(Shop shop, PackedScene shopButtonScene) {
        this.shop = shop;
        this.shopButtonScene = shopButtonScene;
        buttonMenu = new ButtonMenu(64, 0);
    }

    public void OnEnter() {
        foreach (var i in shop.items.Keys) {
            InstantiateButton(shop.items[i]);
        }
    }

    public void OnExit() {
        buttonMenu.Clear();
    }

    private void InstantiateIGButton() {
    }

    private void InstantiateButton(ItemStack itemStack) {
        var newShopButton = (ShopButton)shopButtonScene.Instance();
        newShopButton.Initialize(itemStack.name, itemStack.cost, itemStack.amount);
        newShopButton.OnClickEvent += HandleShopButtonClicked;
        AddChild(newShopButton);
        buttonMenu.Add(newShopButton);
    }

    private void AttemptToBuyItem(string name, int quantity) {
        if (shop.items.ContainsKey(name)) {
            shop.items[name].amount -= quantity;
            if (shop.items[name].amount <= 0) {
                shop.items.Remove(name);
            }
        }
    }

    private void HandleShopButtonClicked(ShopButton button) {
        if (!shop.items.ContainsKey(button.name)) {
            button.Destroy();
        }
        AttemptToBuyItem(button.name, 1);
        if (!shop.items.ContainsKey(button.name)) {
            button.Destroy();
        }
        else
            button.UpdateAmount(shop.items[button.name].amount);
    }
}