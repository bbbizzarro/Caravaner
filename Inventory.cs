using System.Collections.Generic;


public class Inventory {

    int _capacity;
    int _weight;
    Dictionary<string, ItemStack> _items;
    public string Active {private set; get; }

    public Inventory(int capacity) {
        _capacity = capacity; _weight = 0;
        _items = new Dictionary<string, ItemStack>();
    }

    public bool HasItem(string name, int count) {
        return _items.ContainsKey(name) && _items[name].Count >= count;
    }

    public bool HasItem(IItem item) {
        return _items.ContainsKey(item.GetID());
    }

    public IEnumerable<ItemStack> GetItems() {
        return _items.Values;
    }

    public bool TryToAdd(IItem item, int count) {
        if (count <= 0 || 
            _weight + item.GetWeight() * count > _capacity) return false;
        else if (_items.ContainsKey(item.GetID())) 
            _items[item.GetID()].Count += count;
        else 
            _items.Add(item.GetID(), new ItemStack(item, count));
        _weight += item.GetWeight();
        return true;
    }

    public bool TryToRemove(string ID, int count) {
        if (_items.ContainsKey(ID) && _items[ID].Count >= count) {
            _items[ID].Count -= count;
            if (_items[ID].Count <= 0) {
                _items.Remove(ID);
            }
            return true;
        }
        return false;
    }

    public bool TryToRemove(IItem item) {
        if (_items.ContainsKey(item.GetID())) {
            _items[item.GetID()].Count -= 1;
            if (_items[item.GetID()].Count <= 0) {
                _items.Remove(item.GetID());
            }
            return true;
        }
        return false;
    }

    public class ItemStack {
        public IItem Item;
        public int Count;

        public ItemStack(IItem item, int count) {
            Item = item; Count = count;
        }
    }
}


public interface IItem {
    string GetID();
    int GetWeight();
}