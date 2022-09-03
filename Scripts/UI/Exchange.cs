using Godot;
using System;
using System.Collections.Generic;

public class Exchange : MousePoint {
	protected IconData itemBeingSold;
	protected int total = 0;
	Label label;
	Timer timer;
	RandomNumberGenerator rng = new RandomNumberGenerator();
	IconInstancer iconInstancer;
	[Export] string Location = "*";
	[Export] string Category = "*";
	[Export] Rarity Rarity = Rarity.Common;

	private string Export = "None";
	private float ExportFactor = 1;
	private string Import = "None";
	private float ImportFactor = 1;

	private float ItemValue;

	public override void _Ready() {
		base._Ready();
		label = (Label)GetNode("Sprite/Label");
		timer = (Timer)GetNode("Timer");
		timer.Connect("timeout", this, nameof(ResetClickPreview));
		rng.Randomize();
	}

    public override void Initialize(Region region) {
        base.Initialize(region);
		//Category = region.Import;
		SetItem(region.Import);
    }

    public void SetGoods(string import, float importFactor, string export, float exportFactor) {
		Import = import; Export = export; ImportFactor = importFactor; ExportFactor = exportFactor;
		Category = Export;
	}

	protected virtual void SetItem(string export) {
		Export = export;
		rng.Randomize();
		if (rng.Randf() > 0.5f) {
			itemBeingSold = Services.Instance.IconInstancer
				.Select(export, "*", Location, Rarity, -1);
		}
		else {
			itemBeingSold = Services.Instance.IconInstancer
				.Select("Food", "*", Location, Rarity, -1);
		}
		if (itemBeingSold.InCategory(export)) {
			ItemValue = Mathf.Max(0, Mathf.Round(itemBeingSold.value / (rng.Randf() + 2f)));
			GD.Print(ItemValue);
		}
		else {
			ItemValue = itemBeingSold.value;
		}
	}

	protected virtual void AddToValue(IconData input) { 
		if (input.InCategory(Export)) {
			total += Mathf.Max(1, Mathf.RoundToInt(input.value / 3));
		}
		else {
			total += input.value;
		}
	}

	protected override void Preview(bool preview) {
		base.Preview(preview);
		if (preview) { 
			label.Text = 
				String.Format("{0} ({1}/{2})({3})", itemBeingSold.name, total, ItemValue, itemBeingSold.value);
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
		if (total >= ItemValue) { 
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
