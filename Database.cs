using System;
using System.Collections.Generic;
using Godot;

public class Database : ISavable {
    Dictionary<int, Item> records;

    public Database() {
        records = new Dictionary<int, Item>();
    }

    public Item Get(int id) { 
        if (id >= 0 && id < records.Count) 
            return records[id];
        else 
            throw new Exception(String.Format("No record with id: {0} in database.", id));
	}

    public void Add(Item item) {
        records[item.GetID()] = item;
	}

    public void Load(Godot.Collections.Dictionary<string, object> data) {
        foreach (string record in (Godot.Collections.Array)data["records"]) {
            Item newRecord = Item.Default;
			newRecord.Load(JSONUtils.ReadJSON(record));
            records[newRecord.GetID()] = newRecord;
		}
    }

    public Godot.Collections.Dictionary<string, object> Save() {
        List<string> recordSaves = new List<string>();
        foreach (Item record in records.Values) {
            recordSaves.Add(JSON.Print(record.Save()));
		}
        return new Godot.Collections.Dictionary<string, object>() {
            { "records", recordSaves }
        };
    }
}

public struct Item : ISavable {
    [SerializeField] public readonly int id;
    [SerializeField] public readonly string name;
    public static readonly Item Default = new Item(0, "");

    public Item(int id, string name) {
        this.id = id;
        this.name = name;
	}

    public int GetID() {
        return id;
    }

    public void Load(Godot.Collections.Dictionary<string, object> data) {
        JSONUtils.Deserialize(this, data);
    }

    public Godot.Collections.Dictionary<string, object> Save() {
        return JSONUtils.Serialize(this);
    }

    public override string ToString() {
        return name;
    }
}
