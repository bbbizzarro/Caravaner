using System.Collections.Generic;
using Godot;

public class EntityService {
    Node2D _playerNode;
    HashSet<Node2D> _entities = new HashSet<Node2D>();

    public Node2D GetPlayer() {
        return _playerNode;
    }

    public void AddPlayer(Node2D node) {
        _playerNode = node;
        _entities.Add(node);
    }

    public void Add(Node2D node) {
        _entities.Add(node);
    }
    public void Remove(Node2D node) {
        _entities.Remove(node);
    }
}