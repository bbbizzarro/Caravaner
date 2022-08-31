using System;
using System.Collections.Generic;

public class Tile : ISavable, IContainer<int> {
	[SerializeField] public bool visible = false;
	[SerializeField] public int type;
	[SerializeField] public float debugValue;
	[SerializeField] public List<int> items;
	[SerializeField] public int state;
	[SerializeField] public bool open;
	[SerializeField] public bool hasRoad;
	[SerializeField] public string scene = "None";
	[SerializeField] public string tileType;

	public Tile() {
		open = true;
	}

	public Tile(int type, bool visible) {
		this.visible = visible;
		this.type = type;
		items = new List<int>();
	}

	public void SetType(bool isOpen, string tileType) {
		this.tileType = tileType;
		this.open = isOpen;
	}

	public void SetScene(bool isOpen, string tileScene) {
		this.scene = tileScene;
		this.open = isOpen;
	}

	public void Load(Godot.Collections.Dictionary<string, object> data) {
		JSONUtils.Deserialize(this, data);
	}

	public Godot.Collections.Dictionary<string, object> Save() {
		return JSONUtils.Serialize(this);
	}

	public void SetVisible(bool isVisible) {
		visible = isVisible;
	}

    public IEnumerable<int> GetItems() {
		return items;
    }

    public int Remove(int item) {
		items.Remove(item);
		return item;
    }

    public void Add(int item) {
		items.Add(item);
    }

    public void SubscribeToUpdate(ContainerUpdated receiver) {
        throw new NotImplementedException();
    }
}
