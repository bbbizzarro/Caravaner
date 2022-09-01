using Godot;
using System;
using System.Collections.Generic;

public class Exchange : MousePoint {
	protected IconData itemBeingSold;
	protected int total = 0;
	Label label;
	Timer timer;
	RandomNumberGenerator rng;
	IconInstancer iconInstancer;
	[Export] string Location = "Markets";
	[Export] string Category = "*";
	[Export] Rarity Rarity = Rarity.Common;

	private string Export = "None";
	private float ExportFactor = 1;
	private string Import = "None";
	private float ImportFactor = 1;

	public override void _Ready() {
		base._Ready();
		label = (Label)GetNode("Sprite/Label");
		timer = (Timer)GetNode("Timer");
		timer.Connect("timeout", this, nameof(ResetClickPreview));
		rng = new RandomNumberGenerator();
		rng.Randomize();
		SetItem();
	}

	public void SetGoods(string import, float importFactor, string export, float exportFactor) {
		Import = import; Export = export; ImportFactor = importFactor; ExportFactor = exportFactor;
	}

	protected virtual void SetItem() {
		itemBeingSold = Services.Instance.IconInstancer
			.Select(Category, "*", Location, Rarity, -1);
	}

	protected virtual void AddToValue(IconData input) { 
		total += input.value;
	}

	protected override void Preview(bool preview) {
		base.Preview(preview);
		if (preview) { 
			label.Text = 
				String.Format("{0} ({1}/{2})", itemBeingSold.name, total, itemBeingSold.value);
		}
		else { 
			label.Text = "";
		}
	}

	protected override void OnMousePress() {
		//!!!
		return;
		if (itemBeingSold == null) return;
		timer.Start(2f);
		label.Text = 
			String.Format("{0} ({1}/{2})", itemBeingSold.name, total, itemBeingSold.value);
	}

	private void ResetClickPreview() {
		label.Text = "";
	}

	public override bool Add(DragObject dragObject) {
		if (itemBeingSold == null) return false;
		IconData input = Services.Instance.IconInstancer
			.Get(dragObject.GetItemName());
		dragObject.Destroy();
		AddToValue(input);
		if (total >= itemBeingSold.value) { 
			Services.Instance.IconInstancer
				.Place(itemBeingSold, GlobalPosition);
			total = 0;
			return true;
		}
		return false;
	}

	public override bool IsOpen() {
		return true;
	}

}
