using System.Collections.Generic;
using System;
using Godot;

public class TileInstancer {
	private readonly Dictionary<string, string> sceneDB;
    private readonly Dictionary<string, TileData> tileDB;
	private readonly Node parent;
    private readonly string scenePath;
    private readonly string tilePath;

    public TileInstancer(Node parent) {
		this.parent = parent;
        scenePath = "res://Scenes/";
        sceneDB = IndexSceneFiles(scenePath);
        tilePath = "res://caravaner_tile_db.json";
        var csv = new CSV<TileData>();
        tileDB = csv.LoadFromFile(tilePath);
    }

    public IEnumerable<string> GetTileDB() {
        return tileDB.Keys;
	}

    public Node2D SpawnRegionObject(Vector2 globalPosition, string name, Region region ){
        if (sceneDB.ContainsKey(name)) { 
	        PackedScene packedScene = (PackedScene)ResourceLoader.Load(scenePath + sceneDB[name]);
            Node2D node = (Node2D) packedScene.Instance();
            //parent.CallDeferred("add_child", node);/// AddChild(node);
            parent.AddChild(node);
            node.GlobalPosition = globalPosition;
            ((RegionObject)node).Initialize(region);
            return node;
		}
        else { 
			GD.PrintErr(String.Format("Tried to instance tile scene {0} but it doesn't exist.", name));
            return null;
		}
    }

    public Node2D Spawn(Vector2 globalPosition, string name) {
        if (sceneDB.ContainsKey(name)) { 
	        PackedScene packedScene = (PackedScene)ResourceLoader.Load(scenePath + sceneDB[name]);
            Node2D node = (Node2D) packedScene.Instance();
            //parent.CallDeferred("add_child", node);/// AddChild(node);
            parent.AddChild(node);
            node.GlobalPosition = globalPosition;
            return node;
		}
        else { 
			GD.PrintErr(String.Format("Tried to instance tile scene {0} but it doesn't exist.", name));
            return null;
		}
	}

    public Node2D Create(Vector2 globalPosition, string name) {
        TileData tileData = GetTileData(name);
        if (tileData != null && sceneDB.ContainsKey(tileData.scene)) { 
	        PackedScene packedScene = (PackedScene)ResourceLoader.Load(scenePath + sceneDB[tileData.scene]);
            Node2D node = (Node2D) packedScene.Instance();
            parent.AddChild(node);
            node.GlobalPosition = globalPosition;
            SetLabel(node, tileData.name);
            return node;
		}
        else { 
			GD.PrintErr(String.Format("Tried to instance tile scene {0} but it doesn't exist.", tileData.scene));
            return null;
		}
	}

    private TileData GetTileData(string name) { 
        if (tileDB.ContainsKey(name)) {
            return tileDB[name];
		}
        else { 
			GD.PrintErr(String.Format("Tried to get TileData with name {0} but it doesn't exist.", name));
            return null;
		}
	}

    private void SetLabel(Node2D node, string name) {
        try {
            var label = (Label)node.GetNode("Sprite/Label");
            if (name == "Land") { 
                label.Text = "";
			}
            else { 
                label.Text = name;
			}
        }
        catch {
            GD.PrintErr(String.Format("Could not find label on node {0}", node.Name));
		}
	}

    private Dictionary<string, string> IndexSceneFiles(string path) {
        var db = new Dictionary<string, string>();
        var directory = new Directory();
        directory.Open(path);
        directory.ListDirBegin();
        string file = directory.GetNext();
        while (file != "") {
            // Avoid hidden files and grab PNG files only
            if (!file.BeginsWith(".") && file.EndsWith(".tscn")) {
                string name = file.Substring(0, file.Length - 5);
                db.Add(name, file);
			}
            file = directory.GetNext();
		}
        directory.ListDirEnd();
        return db;
	}
}

public class TileData : ISavable, IRecord<string> {
    [SerializeField] public string name;
    [SerializeField] public string scene;

    public string GetKey() {
        return name;
    }

    public void Load(Godot.Collections.Dictionary<string, object> data) {
        JSONUtils.Deserialize(this, data);
    }

    public Godot.Collections.Dictionary<string, object> Save() {
        return JSONUtils.Serialize(this);
    }
}