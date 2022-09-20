using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Caravaner;

public delegate void TileEventHandler(GameWorld.Tile tile);
public class GameWorld {

    public int Width {private set; get;}
    public int Height {private set; get;}
    public event TileEventHandler TileCreatedEvent;

    Tile[,] _world;
    Entity _player;
    TileFactory _tileFactory;
    HashSet<Entity> _entities;

    public GameWorld(int width, int height) {
        Width = width; Height = height;
        _world = new Tile[Width, Height];
        _player = new Entity("Player");
        _tileFactory = new TileFactory();
    }

    public Entity GetPlayer() {
        return _player;
    }

    public void CreateTile(Tile tile) {
        Vector2Int pos = tile.Position;
        if (!IsInBounds(pos.x, pos.y)) return;
        if (_world[pos.x, pos.y] != null) {
            _world[pos.x, pos.y].Destroy();
        }
        _world[pos.x, pos.y] = tile;
        TileCreatedEvent?.Invoke(_world[pos.x, pos.y]);
    }

    public void ReplaceTile(int x, int y, Tile tile) {
        if (!IsInBounds(x, y)) return;
    }

    public void RemoveTile(int x, int y) {
        if (IsInBounds(x, y)) _world[x, y] = null;
    }

    public Tile GetTileAt(int x, int y) {
        if (IsInBounds(x, y)) {
            return _world[x, y];
        }
        return null;
    }

    private bool IsInBounds(int x, int y) {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }

    public delegate void TileChangedEventHandler();
    public delegate void TileDestroyedEventHandler(int x, int y);
    public class Tile {
        public string Name {private set; get; }
        public Vector2Int Position {private set; get; }
        public event TileChangedEventHandler TileChangedEvent;
        public event TileChangedEventHandler TileDestroyedEvent;

        protected GameWorld _gameWorld;

        public Tile(string name, Vector2Int position, GameWorld gameWorld) {
            Name = name;
            Position = position;
            _gameWorld = gameWorld;
        }

        public void Destroy() {
            TileDestroyedEvent?.Invoke();
            _gameWorld.RemoveTile(Position.x, Position.y);
        }

        public override string ToString() {
            return Name;
        }

        public virtual List<string> Preview(Entity entity) {
            return new List<string>(){"None"};
        }

        public virtual void Interact(Entity entity, int option) {
            GD.Print("Interacting.");
        }
    }

    public class Entity {
        public string Name;
        public Vector2Int Position {private set; get; }
        public event EntityChangeHandler EntityChangedEvent;
        public float MovementSpeed = 3f;
        public Inventory Items = new Inventory(10);
        public Inventory Statuses = new Inventory(10);
        public Inventory Efforts = new Inventory(10);

        public Entity(string name) {
            Name = name; 
        }

        public void SetPosition(int x, int y) {
            Position = new Vector2Int(x, y);
            EntityChangedEvent?.Invoke();
        }
    }
}

public class Status : IItem {
    public string Name;
    public Status (string name) {
        Name = name;
    }

    public string GetID() {
        return Name;
    }

    public int GetWeight() {
        return 1;
    }

    public override string ToString() {
        return Name;
    }
}

public delegate void EntityChangeHandler();
