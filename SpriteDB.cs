using System.Collections.Generic;
using Godot;
using System;

public class SpriteDB {
    private string path;
    Dictionary<string, SpriteData> db;
    Dictionary<string, AtlasData> atlases;
	private readonly PackedScene textLabel = (PackedScene)ResourceLoader.Load("res://Scenes/TextLabel.tscn");
    private Dictionary<string, AtlasTexture> sprites;

    public SpriteDB() {
        path = "res://Sprites/";
        IndexPNGFiles(path);
        IndexAtlasFiles("res://Sprites/AtlasTextures/");
    }

    public StreamTexture Get(string name) { 
        if (db.ContainsKey(name)) {
            return ResourceLoader.Load<StreamTexture>(path + db[name].filename);
		}
        else {
            GD.PrintErr(String.Format("Could not find sprite {0} in database.", name));
            return null;
		}
	}

    public AtlasTexture GetAtlasTexture(string name) {
        if (sprites.ContainsKey(name)) {
            return sprites[name];
        }
        else {
            GD.PrintErr(String.Format("Could not find sprite {0} in database.", name));
            return null;
        }
    }

    public void SetTextLabel(string name, Sprite sprite) {
        sprite.Texture = null;
        Label label = (Label)textLabel.Instance();
        label.Text = name;
        sprite.AddChild(label);
	}

    public void SetTexture(string name, Sprite sprite) {
        IconData iconData = Services.Instance.IconInstancer.Get(name);
        if (iconData != null) { 
            SetTextLabel(String.Format("{0} ({1})",iconData.name, iconData.value), sprite);
		}
        else { 
            SetTextLabel(name, sprite);
		}
        return;

        if (atlases.ContainsKey(db[name].atlas)) {
            LoadAtlasSprite(name, sprite);
		}
        else {
            var texture = Get(name);
            sprite.Texture = texture;
		}

	}
    private void LoadAtlasSprite(string name, Sprite sprite) { 
        AtlasData atlas = atlases[db[name].atlas];
        sprite.Texture = atlas.texture;
        sprite.Hframes = atlas.hframes;
        sprite.Vframes = atlas.vframes;
        sprite.Frame = db[name].frame;
	}

    private void IndexAtlasFiles(string path) {
        sprites = new Dictionary<string, AtlasTexture>();

        var directory = new Directory();
        directory.Open(path);
        directory.ListDirBegin();

        string file = directory.GetNext();
        while (file != "") {
            // Avoid hidden files and grab PNG files only
            if (!file.BeginsWith(".") && file.EndsWith(".tres")) {
                string name = file.Substring(0, file.Length - 5);
                //db.Add(name, new SpriteData(file, 0, ""));
                sprites.Add(name, (AtlasTexture)ResourceLoader.Load(path + file));
			}
            file = directory.GetNext();
		}

        directory.ListDirEnd();
	}
    private void IndexPNGFiles(string path) {
        db = new Dictionary<string, SpriteData>();
        atlases = new Dictionary<string, AtlasData>();

        var directory = new Directory();
        directory.Open(path);
        directory.ListDirBegin();

        string file = directory.GetNext();
        while (file != "") {
            // Avoid hidden files and grab PNG files only
            if (!file.BeginsWith(".") && file.EndsWith(".png")) {
                if (file.BeginsWith("Atlas")) {
                    ParseAtlas(file, path);
				}
                else { 
                    string name = file.Substring(0, file.Length - 4);
                    db.Add(name, new SpriteData(file, 0, ""));
				}
			}
            file = directory.GetNext();
		}

        directory.ListDirEnd();
	}

    private void ParseAtlas(string fileName, string path) { 
        // Atlas000x000Name.png
        int hframes; int vframes;
        if (!Int32.TryParse(fileName.Substring(5, 3), out hframes)
         || !Int32.TryParse(fileName.Substring(9, 3), out vframes)) {
            GD.PrintErr(String.Format("Could not parse atlas {0}", fileName));
		}
        else {
            string atlasName = fileName.Substring(12, fileName.Length - 16);
            for (int row = 0; row < hframes; ++row) { 
                for (int col = 0; col < vframes; ++col) {
                    int frame = row * vframes + col;
                    string atlasSubName = String.Format("{0}{1}", atlasName, frame);
                    db.Add(atlasSubName,
                        new SpriteData(
                            fileName,
                            frame,
                            atlasName
							) 
                        );
				}
			}
            atlases.Add(atlasName,
                        new AtlasData(
                            ResourceLoader.Load<StreamTexture>(path + fileName),
                            hframes,
                            vframes));
		}
	}
}

public struct AtlasData {
    public StreamTexture texture;
    public int hframes;
    public int vframes;
    public AtlasData(StreamTexture texture, int hframes, int vframes) {
        this.texture = texture;
        this.hframes = hframes;
        this.vframes = vframes;
	}
}

public struct SpriteData {
    public string filename;
    public int frame;
    public string atlas;
    public SpriteData(string filename, int frame, string atlas) {
        this.filename = filename;
        this.frame = frame;
        this.atlas = atlas;
	}
}
