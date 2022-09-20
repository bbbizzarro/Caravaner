using Godot;
using Caravaner;

public class WorldMapRenderer {
	WorldMap _worldMap;
	int _pixelsPerUnit;
	PackedScene viewScene = (PackedScene)ResourceLoader.Load("res://Scenes/View.tscn");
	Node _root;

	public WorldMapRenderer(WorldMap worldMap, int pixelsPerUnit, Node root) {
		_worldMap = worldMap;
		_worldMap.EntityCreatedEvent += InstantiateEntity;
		_pixelsPerUnit = pixelsPerUnit;
		_root = root;
	}

   	public AtlasTexture GetSprite(string name) {
   	    try {
   	        return ResourceLoader.Load<AtlasTexture>("res://Sprites/AtlasTextures/" + name + ".tres");
   	    }
   	    catch {
   	        return null;
   	    }
   	}

	public void InstantiateEntity(Entity entity) {
		var n = (Interactable)viewScene.Instance();
		_root.AddChild(n);
		n.Init(entity, this);
	}

	public Vector2 GridToWorld(Vector2Int pos) {
		return new Vector2(_pixelsPerUnit * pos.x, -_pixelsPerUnit * pos.y);
	}
   	public Vector2Int WorldToGrid(Vector2 pos) {
   	    return new Vector2Int(Mathf.RoundToInt(pos.x / _pixelsPerUnit),
   	                          Mathf.RoundToInt(Mathf.Abs(pos.y) / _pixelsPerUnit));
   	}
}