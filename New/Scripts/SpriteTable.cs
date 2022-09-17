using System.Collections.Generic;
using Godot;

public class SpriteTable {

    Dictionary<string, int> db;

    public SpriteTable() {
        db = new Dictionary<string, int>() {
            {"Dirt", 55}, {"Grass", 91}, {"GrassItem", 129},
            {"Tree", 80}
        };
    }

    public int Get(string id) {
        if (id != null && db.ContainsKey(id)) {
            return db[id];
        }
        else {
            return 19;
        }
    }
}