using System.Collections.Generic;
using Godot;
using System;

public class SpriteDB {
    private string path;
    Dictionary<string, SpriteData> db;

    public SpriteDB() {
        path = "res://Sprites/";
        db = IndexPNGFiles(path);
    }

    public StreamTexture Get(string name) { 
        if (db.ContainsKey(name)) {
            GD.Print(path + db[name].filename);
            return ResourceLoader.Load<StreamTexture>(path + db[name].filename);
		}
        else {
            GD.PrintErr(String.Format("Could not find sprite {0} in database.", name));
            return null;
		}
	}

    private Dictionary<string, SpriteData> IndexPNGFiles(string path) {
        var db = new Dictionary<string, SpriteData>();
        var directory = new Directory();
        directory.Open(path);
        directory.ListDirBegin();
        string file = directory.GetNext();
        while (file != "") {
            // Avoid hidden files and grab PNG files only
            if (!file.BeginsWith(".") && file.EndsWith(".png")) {
                string name = file.Substring(0, file.Length - 4);
                db.Add(name, new SpriteData(file));
			}
            file = directory.GetNext();
		}
        directory.ListDirEnd();
        return db;
	}
}

public struct SpriteData {
    public string filename;
    public SpriteData(string filename) {
        this.filename = filename;
	}
}
