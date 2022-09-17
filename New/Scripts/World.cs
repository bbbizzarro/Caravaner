using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Caravaner;

public class GameWorld {

    public int Width {private set; get;}
    public int Height {private set; get;}
    Tile[,] _world;
    Entity _player;
    HashSet<Entity> _entities;

    public GameWorld(int width, int height) {
        Width = width; Height = height;
        _world = new Tile[width, height];
        for (int x = 0; x < width; ++x) {
            for (int y = 0; y < height; ++y) {
                _world[x, y] = new Tile("Dirt");
            }
        }
        _player = new Entity("Player");
    }

    public Entity GetPlayer() {
        return _player;
    }

    public void AddPropAt(int x, int y, Prop prop) {
        if (IsInBounds(x, y)) {
            _world[x,y].AddProp(prop);
        }
    }

    public IEnumerable<Prop> GetPropsAt(int x, int y) {
        if (IsInBounds(x, y)) {
            return _world[x,y].GetProps();
        }
        return null;
    }

    public void PlacePropAtNearestOpenTile(int x, int y, Prop prop) {
        Tile tile = GetTileAt(x, y);
        if (tile != null && tile.IsOpen()) {
            tile.AddProp(prop);
            return;
        }
        for (int i = -1; i <= 1; ++i) {
            for (int j = -1; j <= 1; ++j) {
                tile = GetTileAt(x + i, y + j);
                if (tile != null && tile.IsOpen()) {
                    tile.AddProp(prop);
                    return;
                }
            }
        }
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

    public class Item {
        public string Name;
    }

    public class Prop {
        public string Name;
        public bool SharesTile = true;

        public event PropChangedHandler DestroyEvent;

        public virtual List<Option> GetOptions(Entity entity) {
            var options = new List<Option>();
            return options;
        }

        public virtual string Preview() {
            return "";
        }

        public virtual void Interact() {
        }

        public void Destroy() {
            DestroyEvent?.Invoke(this);
        }
    }

    public class HarvestableProp : Prop {
        Vector2Int _position;
        GameWorld _gameWorld;
        RandomNumberGenerator _rng;

        public HarvestableProp(string name, Vector2Int position, GameWorld gameWorld) {
            Name = name;
            _position = position;
            _gameWorld = gameWorld;
            _rng = new RandomNumberGenerator();
            _rng.Randomize();
        }

        public override string Preview() {
            return "Harvest " + Name;
        }

        public override void Interact() {
            for (int i = 0; i < _rng.RandiRange(0, 3); ++i) {
                _gameWorld.PlacePropAtNearestOpenTile(
                    _position.x, _position.y,
                    new ItemProp("GrassItem")
                );
            }
            Destroy();
        }
    }

    public class ItemProp : Prop {

        public ItemProp(string name) {
            Name = name;
            SharesTile = false;
        }

        public override string Preview() {
            return "Pick up " + Name;
        }

        public override void Interact() {
            Destroy();
        }
    }

    public class Tile {
        public string Name;
        List<Prop> _props;

        public event ViewableChangedHandler TileChangedEvent;

        public Tile(string name) {
            Name = name;
            _props = new List<Prop>();
        }

        public bool IsOpen() {
            foreach (var prop in _props) {
                if (!prop.SharesTile) return false;
            }
            return true;
        }

        public IEnumerable<Prop> GetProps() {
            return _props;
        }

        public void AddProp(Prop prop) {
            prop.DestroyEvent += RemoveProp; 
            _props.Add(prop);
            TileChangedEvent?.Invoke();
        }

        public void RemoveProp(Prop prop) {
            _props.Remove(prop);
            TileChangedEvent?.Invoke();
        }

        public Prop GetTopProp() {
            if (_props.Count > 0) {
                return _props[_props.Count - 1];
            }
            return null;
        }

        public override string ToString() {
            return Name;
        }
    }

    public class Entity {
        public string Name;
        public Vector2Int Position {private set; get; }
        public event EntityChangeHandler EntityChangedEvent;
        public float MovementSpeed = 3f;

        public Entity(string name) {
            Name = name; 
        }

        public void SetPosition(int x, int y) {
            Position = new Vector2Int(x, y);
            EntityChangedEvent?.Invoke();
        }
    }
}

public delegate void PropChangedHandler(GameWorld.Prop prop);
public delegate void ViewableChangedHandler();
public delegate void EntityChangeHandler();

public class WorldGenerator {
    RandomNumberGenerator _rng;
    public WorldGenerator() {
        _rng = new RandomNumberGenerator();
        _rng.Randomize();
    }

    public GameWorld Generate(int width, int height) {
        GameWorld gameWorld = new GameWorld(width, height);
        var terrainTypes = new List<string>() {"Dirt", "Grass"};
        for (int x = 0; x < width; ++x) {
            for (int y = 0; y < height; ++y) {
                var tile = gameWorld.GetTileAt(x, y);
                if (tile != null) {
                    if (_rng.Randf() > 0.5f) {
                        gameWorld.AddPropAt(x, y,
                            new GameWorld.HarvestableProp("Grass", new Vector2Int(x, y), gameWorld));
                    }
                    else if (_rng.Randf() > 0.9f) {
                        gameWorld.AddPropAt(x, y,
                            new GameWorld.HarvestableProp("Tree", new Vector2Int(x, y), gameWorld));

                    }
                }
            }
        }
        return gameWorld;
    }
}