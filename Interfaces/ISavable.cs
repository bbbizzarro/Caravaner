using Godot;
using System;

public interface ISavable {
    Godot.Collections.Dictionary<string, object> Save();
    void Load(Godot.Collections.Dictionary<string, object> data);
}
