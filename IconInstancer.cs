using System.Collections.Generic;
using System;
using Godot;
public class IconInstancer {
	private readonly PackedScene iconScene = (PackedScene)ResourceLoader.Load("res://DragObject.tscn");
	private readonly Dictionary<string, IconData> db;
	private readonly Node parent;
	private RandomNumberGenerator rng = new RandomNumberGenerator();

	public IconInstancer(Node parent) {
		rng.Randomize();
		this.parent = parent;
		db = new Dictionary<string, IconData>() {
			{"Building",  new IconData("Building", "Icon0", 1)},
			{"Land",  new IconData("Land", "Icon1", 1)},
			{"Item",  new IconData("Item", "Icon2", 0)}
		};
	}

	public DragObject CreateRandom(Vector2 globalPosition) {
		List<string> keys = new List<string>(db.Keys);
		return Create(globalPosition, keys[rng.RandiRange(0, keys.Count-1)]);
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

public struct IconData {
	public string name;
	public string sprite;
	public int type;
	public IconData(string name, string sprite, int type) {
		this.name = name;
		this.sprite = sprite;
		this.type = type;
	}
}
