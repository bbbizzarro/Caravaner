using Godot;
using System;
using System.Collections.Generic;

public class UI : Node2D {
	private World world;
	private IContainer<int> player;
	private Database database;
	[Signal] delegate void PlayerContainerUpdated(IEnumerable<Item> items);
	private Hotbar hotbar;
	[Signal] delegate void DisplayItem(int x, int y, string title);
	Label clock;
	TextureProgress energyHUD;
	PlayerData playerData;
	private const float MaxRadialDegrees = 360f;
	Label TerminalLabel;

	public void Initialize(World world, IContainer<int> player) {
		this.world = world;
		this.player = player;
		DEBUGInitDB();
		player.SubscribeToUpdate(UpdatePlayerUI);
		hotbar = (Hotbar)GetNode("CanvasLayer/MarginContainer/PanelContainer/Hotbar");
		clock = (Label)GetNode("CanvasLayer/Clock/Label");
		UpdatePlayerUI();
		energyHUD = (TextureProgress)GetNode("CanvasLayer/EnergyHud");
		playerData = Services.Instance.PlayerData;
		playerData.OnEnergySet += SetEnergyHudValue;
		TerminalLabel = (Label)GetNode("CanvasLayer/Terminal/Label");
	}

	public void UpdateTerminalText(string text) {
		TerminalLabel.Text = text;
	}

	public void PlayerInformationToText(Player player) {
		string text = "INV: ";
		if (player == null || player.Inventory == null) return;
		foreach (Inventory.ItemStack item in player.Inventory.GetItems() ) {
			text += String.Format("[{0}(x{1})] ", item.Item.GetID(), item.Count);
		}
		UpdateTerminalText(text);
	}

	public void SetEnergyHudValue(float value) {
		energyHUD.RadialFillDegrees 
			= playerData.EnergyPercent * MaxRadialDegrees;
	}

	public override void _Process(float delta) {
		//base._Process(delta);
		//if (Input.IsActionJustPressed("interact")) {
		//	GD.Print(FormatItems(player.GetItems()));
		//}
		HandlePointerClick();
		if (Services.Instance != null) {
			clock.Text = Services.Instance.WorldState.TimeToString();
		}
	}

	private void HandlePointerClick() { 
		if (Input.IsActionJustPressed("ui_select")) {
			Vector2 mousePos = GetGlobalMousePosition();
			Tile tile = world.GetTile(mousePos);
			if (tile != null) {
				var tileItems = new List<int>(tile.GetItems());
				if (tileItems.Count > 0) {
					Item item = database.Get(tileItems[0]);
					EmitSignal("DisplayItem", mousePos.x, mousePos.y, item.name);
					//int item = tile.Remove(tileItems[0]);
					//player.Add(item);
					//GD.Print(String.Format("Obtained {0}!", database.Get(item)));
				}
				//GD.Print(FormatItems(tile.GetItems()));
			}
		}
	}

	private void UpdatePlayerUI() {
		hotbar.OnPlayerContainerUpdated(GetItemsFromIds(player.GetItems()));
		GD.Print(FormatItems(player.GetItems()));
		//EmitSignal(nameof(PlayerContainerUpdated), GetItemsFromIds(player.GetItems()));
	}

	private IEnumerable<Item> GetItemsFromIds(IEnumerable<int> ids) {
		List<Item> items = new List<Item>();
		foreach (int id in ids) {
			items.Add(database.Get(id));
		}
		return items;
	}

	private string FormatItems(IEnumerable<int> items) {
		string sm = "[";
		foreach (int id in items) {
			sm += String.Format(" {0} ", database.Get(id));
		}
		sm += "]";
		return sm;
	}

	private void DEBUGInitDB() {
		// Too specific?
		database = new Database();
		database.Add(new Item(0, "Martial Training", "STR", 1));
		database.Add(new Item(1, "Olympic Champion", "STR", 3));
		database.Add(new Item(2, "Read: Gant's Almanac", "INT", 1));
		database.Add(new Item(3, "Visited: Amon's Library", "INT", 2));
		database.Add(new Item(4, "Dreamt: A Better Future", "WIL", 2));
		database.Add(new Item(5, "Toxic Water Reservoir", "NON", 0));
	}
}


public class Interactor { 
	public Interactor() { 
	}
}

public interface Interaction {

}

public class ResourceTile { 

	public int GetNextState(int currState) {
		return 0;
	}

	public string GetReqName() {
		return "STR";
	}
	public int GetReqAmt() {
		return 1;
	}
}
