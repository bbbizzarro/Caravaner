using Godot;
using System;
using System.Collections.Generic;

public class EntityInventory  {
    Dictionary<string, int> _inventory;

    public EntityInventory() {
        _inventory = new Dictionary<string, int>();
    }

    public void Add(string id, int amount) {
        if (!_inventory.ContainsKey(id)) {
            _inventory.Add(id, amount);
        }
        else {
            _inventory[id] = _inventory[id] + amount;
        }
    }

    public IEnumerable<string> GetItemIds() {
        return _inventory.Keys;
    }

    public int GetCount(string id) {
        if (!_inventory.ContainsKey(id)) return 0;
        else return _inventory[id];
    }
}
