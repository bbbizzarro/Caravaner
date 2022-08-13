using System.Collections.Generic;
using System;
using Godot;

public class TileInstancer {
	private readonly Dictionary<string, string> db;
	private readonly Node parent;
    private readonly string path;

    public TileInstancer(Node parent) {
		this.parent = parent;
        path = "res://Scenes/";
        db = IndexSceneFiles(path);
    }

    public Node2D Create(Vector2 globalPosition, string name) { 
        if (db.ContainsKey(name)) { 
	        PackedScene packedScene = (PackedScene)ResourceLoader.Load(path + db[name]);
            Node2D node = (Node2D) packedScene.Instance();
            parent.AddChild(node);
            node.GlobalPosition = globalPosition;
            return node;
		}
        else { 
			GD.PrintErr(String.Format("Tried to instance tile scene {0} but it doesn't exist.", name));
            return null;
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
