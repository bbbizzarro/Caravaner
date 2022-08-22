using System.Collections.Generic;
using System;
using Godot;
using System.Linq;
public class IconInstancer {
	private readonly PackedScene iconScene = (PackedScene)ResourceLoader.Load("res://DragObject.tscn");
	private readonly Dictionary<string, IconData> db;
	private readonly Node parent;
	private readonly RandomNumberGenerator rng = new RandomNumberGenerator();
	private readonly Rarity[] rarities = new Rarity[]
			{Rarity.Common,   Rarity.Uncommon,  Rarity.Rare,
			 Rarity.Singular, Rarity.Legendary, Rarity.Ludicrous};
	private readonly CategoryTree categoryTree;

	public IconInstancer(Node parent) {
		rng.Randomize();
		this.parent = parent;
		db = new CSV<IconData>().LoadFromFile("res://caravaner_icon_db.json");
		categoryTree = new CategoryTree("res://category_db.txt");
	}

	public Rarity Roll(Rarity minimumRarity) {
		return Roll(100 - (int)minimumRarity);
	}

	public Rarity Roll(int modifier) {
		int roll = Mathf.Clamp(rng.RandiRange(0, 100) + modifier, 0, 100);
		int curr = 0;
		foreach (Rarity rarity in rarities) { 
			curr += (int)rarity;
			if (roll <= curr) {
				GD.Print(rarity);
				return rarity;
			}
		}
		GD.PrintErr("Invalid rarity. rolled");
		return Rarity.Common;
	}

	public IconData Get(string name) {
		if (!db.ContainsKey(name)) return null;
		return db[name];
	}

	public IconData Select(string category, string subcategory, 
							string material, string state, 
							string location, Rarity rarity, int value) {
		if (rarity == Rarity.Any) rarity = Roll(0);
		var options
			= db.Values.Where(i => IsString(i.subcategory, subcategory) &&
								   InCategory(i.material, material) &&
								   IsString(i.state, state) &&
								   IsString(i.location, location) &&
								   IsRarity(i.rarity, rarity) &&
								   IsValue(i.value, value))
					   .Where(i => InCategory(i.category, category)).ToList();

		if (options.Count == 0) return null;
		return options[rng.RandiRange(0, options.Count - 1)];
	}

	public DragObject Spawn(string name, Vector2 globalPosition) { 
		if (db.ContainsKey(name)) {
			var icon = (DragObject)iconScene.Instance();
			parent.AddChild(icon);
			icon.Set(name, db[name].type);
			Services.Instance.SpriteDB.SetTexture(db[name].sprite, icon.GetSprite());
			icon.GlobalPosition = globalPosition;
			return icon;
		}
		else {
			GD.PrintErr(String.Format("Tried to instance icon {0} but it doesn't exist.", name));
			return null;
		}
	}

	public List<IconData> SelectMany(int count, string category, string subcategory,
									 string material, string state,
									 string location, Rarity rarity, int value) {
		var icons = new List<IconData>();
		for (int i = 0; i < count; ++i) {
			var iconData = Select(category, subcategory, material, state, location, rarity, value);
			if (iconData != null) {
				icons.Add(iconData);
			}
		}
		return icons;
	}

	public void SpawnGroup(Vector2 centerPosition, IEnumerable<IconData> icons) {
		var iconList = icons.ToList();
		if (iconList.Count == 0) return;
		else if (iconList.Count == 1) {
			Place(rng.RandfRange(-0.5f, 0.5f), iconList[0], centerPosition);
			return;
		}
		else if (iconList.Count == 2) { 
			Place(rng.RandfRange(-1f, -0.2f), iconList[0], centerPosition);
			Place(rng.RandfRange(0.2f, 1f), iconList[1], centerPosition);
			return;
		}

		float delta = 2f / (iconList.Count - 1f);
		float start = -1f;
		for (int i = 0; i < iconList.Count; ++i) { 
			Place(start, iconList[i], centerPosition);
			start += delta;
		}
	}

	public void Place(string name, Vector2 centerPosition) {
		Place(rng.RandfRange(-0.5f, 0.5f), Get(name), centerPosition);
	}

	public void Place(IconData iconData, Vector2 centerPosition) {
		Place(rng.RandfRange(-0.5f, 0.5f), iconData, centerPosition);
	}

	public void Place(float x, IconData iconData, Vector2 centerPosition) {
		DragObject dragObject = Spawn(iconData.name, centerPosition);
		if (dragObject == null) return;
		Vector2 direction = new Vector2(x, -rng.RandfRange(0.2f, 1f)).Normalized();
		dragObject.Initialize(centerPosition, direction);
	}

	private bool IsRarity(Rarity candidate, Rarity required) {
		return required == Rarity.None || candidate >= required;
	}

	private bool IsString(string candidate, string required) {
		if (!required.Contains("|")) { 
			return required == "*" || candidate == required;
		}
		foreach (string category in required.Split('|')) { 
			if (category == candidate) {
				return true;
			}
		}
		return false;
	}

	public bool InCategory(string candidate, string required) {
		if (!required.Contains("|")) {
			return required == "*" || categoryTree.InCategory(candidate, required);
		}
		foreach (string category in required.Split('|')) {
			if (categoryTree.InCategory(candidate, category)) {
				return true;
			}
		}
		return false;
	}

	private bool IsValue(int candidate, int required) {
		return required < 0 || candidate == required;
	}


}

public class IconData : ISavable, IRecord<string> {
	[SerializeField] public string name;
	[SerializeField] public string sprite;
	[SerializeField] public int type;
	[SerializeField] public string category;
	[SerializeField] public string subcategory;
	[SerializeField] public string material;
	[SerializeField] public string state;
	[SerializeField] public string location;
	public Rarity rarity; 
	[SerializeField] public int value;

	public IconData() {}

	public string GetKey() {
		return name;
	}

	public bool HasState(string state) {
		return this.state == state;
	}

	public bool InCategory(string category) {
		return Services.Instance.IconInstancer
			.InCategory(this.category, category);
	}
	public bool HasMaterial(string material) {
		return Services.Instance.IconInstancer
			.InCategory(this.material, material);
	}

	public void Load(Godot.Collections.Dictionary<string, object> data) {
		Enum.TryParse((string)data["rarity"], out rarity);
		JSONUtils.Deserialize(this, data);
	}

	public Godot.Collections.Dictionary<string, object> Save() {
		var data = JSONUtils.Serialize(this);
		data["rarity"] = rarity.ToString();
		return data;
	}
}

public enum Rarity { 
	Common = 50,
	Uncommon = 30,
	Rare = 10,
	Singular = 6,
	Legendary = 3,
	Ludicrous = 1,
	Unique = 0,
	None = 0,
	Any = 0,
}
