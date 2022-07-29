using System.Collections.Generic;
using Godot;

public class Pool<T> : IPool<T> where T : IActive {

    private readonly Stack<T> active;
    private readonly Stack<T> inactive;

    public Pool() {
        active = new Stack<T>();
        inactive = new Stack<T>();
    }

    public bool IsEmpty() {
        return inactive.Count <= 0;
    }

    public void Add(T item) {
        inactive.Push(item);
        item.SetActive(false);
    }

    public void Clear() {
        T nextActive;
        while (active.Count > 0) {
            nextActive = active.Pop();
            nextActive.SetActive(false);
            inactive.Push(nextActive);
        }
    }

    public T Get() {
        if (inactive == null || inactive.Count <= 0) {
            GD.PrintErr("No objects in pool.");
            return default;
        }
        T next = inactive.Pop();
        next.SetActive(true);
        active.Push(next);
        return next;
    }

    public IEnumerable<T> GetActive() {
        return active;
    }
}
