using System;

public class Tile : ISavable {
	[SerializeField] public bool visible = false;
	[SerializeField] public int type;
	[SerializeField] public float debugValue;

	public Tile(int type, bool visible) {
		this.visible = visible;
		this.type = type;
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
}
