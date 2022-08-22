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

	//!!! Should save
	private HashSet<string> InstancedUniques;

	public IconInstancer(Node parent) {
		rng.Randomize();
		this.parent = parent;
		db = new CSV<IconData>().LoadFromFile("res://caravaner_icon_db.json");
		categoryTree = new CategoryTree("res://category_db.txt");
		InstancedUniques = new HashSet<string>();
	}

	public Rarity Roll(Rarity minimumRarity) {
		int rarityModifier = 0;
		for (int i = 1; i < rarities.Count(); ++i) { 
			if (minimumRarity == rarities[i - 1])
				return Roll(rarityModifier);
			rarityModifier += (int)rarities[i - 1];
		}
		return Roll(rarityModifier);
	}

	public Rarity Roll(int modifier) {
		int roll = Mathf.Clamp(rng.RandiRange(modifier, 100), 1, 100);
		int curr = 1;
		foreach (Rarity rarity in rarities) { 
			curr += (int)rarity;
			if (roll < curr) {
				GD.Print(String.Format("MOD: {0}, ROLL: {1}, RARITY: {2}", modifier, roll, rarity));
				return rarity;
			}
		}
		//GD.PrintErr("Invalid rarity. rolled");
		GD.Print(String.Format("MOD: {0}, ROLL: {1}, RARITY: {2}", modifier, roll, Rarity.Ludicrous));
		return Rarity.Ludicrous;
	}

	public IconData Get(string name) {
		if (!db.ContainsKey(name)) return null;
		return db[name];
	}

	public IconData Select(string category, string material,
						   string location, Rarity rarity, int value) {
		if (rarity != Rarity.Any) 
			rarity = Roll(rarity);
		var options
			= db.Values.Where(i => IsString(i.location, location) &&
								   IsValue(i.value, value))
					   .Where(i => InCategory(i.material, material) && 
								   InCategory(i.category, category));
		// Get option at least as rare as
		List<IconData> optionList;
		int startingRarity = 0;
		for (int i = rarities.Length - 1; i >= 0; --i) {
			if (rarities[i] < rarity) {
				startingRarity = i;
			}
			else if (options.Count(x => IsRarity(x.rarity, rarities[i])) > 0) {
				optionList = options.Where(x => IsRarity(x.rarity, rarities[i])).ToList();
				return optionList[rng.RandiRange(0, optionList.Count - 1)];
			};
		}
		// If not such option exists in database get even rarer option
		for (int i = startingRarity; i < rarities.Length; ++i) {
			if (options.Count(x => IsRarity(x.rarity, rarities[i])) > 0) {
				optionList = options.Where(x => IsRarity(x.rarity, rarities[i])).ToList();
				return optionList[rng.RandiRange(0, optionList.Count - 1)];
			};
		}
		GD.PrintErr(String.Format("No such icon with select parameters: {0} {1} {2} {3} {4}",
			category, material, location, rarity, value));
		return null;
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

	public List<IconData> SelectMany(int count, string category, 
									 string material, string location, 
									 Rarity rarity, int value) {
		var icons = new List<IconData>();
		for (int i = 0; i < count; ++i) {
			var iconData = Select(category, material, location, rarity, value);
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
		return required == Rarity.Any || candidate == required;
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
	[SerializeField] public string material;
	[SerializeField] public string location;
	public Rarity rarity; 
	[SerializeField] public int value;
	[SerializeField] public bool isUnique;

	public IconData() {}

	public string GetKey() {
		return name;
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
		//isUnique = (string)data["unique"] == "TRUE";
		JSONUtils.Deserialize(this, data);
	}

	public Godot.Collections.Dictionary<string, object> Save() {
		var data = JSONUtils.Serialize(this);
		data["rarity"] = rarity.ToString();
		//data["unique"] = isUnique ? "TRUE" : "FALSE";
		return data;
	}
}

public enum Rarity { 
	Common = 60,   // 1 60
	Uncommon = 20, // 61 80
	Rare = 10,     // 81, 90
	Singular = 6,  // 91, 96
	Legendary = 3, // 97, 99
	Ludicrous = 1, // 100
	Unique = 0,
	Any = 0
}
