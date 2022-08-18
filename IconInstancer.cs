using System.Collections.Generic;
using System;
using Godot;
using System.Linq;
public class IconInstancer {
	private readonly PackedScene iconScene = (PackedScene)ResourceLoader.Load("res://DragObject.tscn");
	private readonly Dictionary<string, IconData> db;
	private readonly Node parent;
	private RandomNumberGenerator rng = new RandomNumberGenerator();

	public IconInstancer(Node parent) {
		rng.Randomize();
		this.parent = parent;
		var csv = new CSV<IconData>();
		db = csv.LoadFromFile("res://caravaner_icon_db.json");
	}

	public IconData GetData(string name) { 
		if (db.ContainsKey(name)) {
			return db[name];
		}
		else {
			GD.PrintErr("Icon with name {0} not in database.", name);
			return null;
		}
	}

	public DragObject CreateFromCategory(Vector2 globalPosition, string category) {
		int randValue = rng.RandiRange(0, 100);
		List<IconData> options;
		if (category != "") 
			options = db.Values.Where(a => a.InCategory(category) &&
									  a.rarity <= randValue).ToList();
		else 
			options = db.Values.Where(a => a.rarity <= randValue).ToList();

		if (options.Count != 0) { 
			return Create(globalPosition,
				options[rng.RandiRange(0, options.Count - 1)].name);
		}
		else return null;
	}

	public DragObject CreateRandom(Vector2 globalPosition) {
		int randValue = rng.RandiRange(0, 100);
		List<IconData> options = db.Values.Where(a => a.rarity <= randValue).ToList();
		if (options.Count != 0) { 
			return Create(globalPosition,
				options[rng.RandiRange(0, options.Count - 1)].name);
		}
		else return null;
	}

	public DragObject Create(Vector2 globalPosition, string name) { 
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
}

public class IconData : ISavable, IRecord<string> {
	[SerializeField] public string name;
	[SerializeField] public string sprite;
	[SerializeField] public int type;
	[SerializeField] public int rarity; // Rarer than RARITY% of items
	public HashSet<string> categories = new HashSet<string>();

	public IconData() {}

	public string GetKey() {
		return name;
	}

	public IEnumerable<string> GetCategories() {
		return categories;
	}

	public bool InCategory(string category) {
		return categories.Contains(category);
	}

	private void LoadCategories(Godot.Collections.Dictionary<string, object> data) {
		categories = new HashSet<string>();
		if (data.ContainsKey("categories")) {
			foreach (string category in ((string)data["categories"]).Split(",")) {
				categories.Add(category);
			}
		}
	}

	private Godot.Collections.Dictionary<string, object> SaveCategories(Godot.Collections.Dictionary<string, object> data) {
		data["categories"] = String.Join(",", categories);
		return data;
	}

	public void Load(Godot.Collections.Dictionary<string, object> data) {
		JSONUtils.Deserialize(this, data);
		LoadCategories(data);
	}

	public Godot.Collections.Dictionary<string, object> Save() {
		return SaveCategories(JSONUtils.Serialize(this));
	}
}
