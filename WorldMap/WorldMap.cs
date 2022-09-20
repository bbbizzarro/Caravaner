using Caravaner;
using Godot;
using System.Collections.Generic;

public delegate void HandleEntityCreation(Entity entity);

public class WorldMap {

	public int Width {private set; get; }
	public int Height {private set; get; }
	public event HandleEntityCreation EntityCreatedEvent;

    // Static entities
    MapTile[,] _map;

	public WorldMap(int width, int height) {
		Width = width; Height = height;
		_map = new MapTile[width, height];
        for (int x = 0; x < Width; ++x) {
            for (int y = 0; y < Height; ++y) {
                _map[x,y] = new MapTile();
            }
        }
	}

    // Generation
    // =====================================================
	public void Seed(int count) {
		var openPoints = new RandList<Vector2Int>(GetOpenPoints());
		for (int i = 0 ; i < count; ++i) {
			if (openPoints.Count > 0) {
				var point = openPoints.Pop();
				CreateEntity(new PlantEntity("Plant", new Vector2Int(point.x, point.y), this));
			}
		}
	}

    // Entity Handling
    // =====================================================
	public void CreateEntity(Entity entity) {
		Vector2Int pos = entity.Position;
		if (!IsValid(pos.x, pos.y)) return;
		if (HasEntity(pos.x, pos.y)) DestroyEntity(GetEntityAt(pos.x, pos.y));
		_map[pos.x, pos.y].Entity = entity;
		entity.EntityDestroyedEvent += DestroyEntity;
		EntityCreatedEvent?.Invoke(entity);
	}

	public void DestroyEntity(Entity entity) {
		Vector2Int pos = entity.Position;
		if (!IsValid(pos.x, pos.y)) return;
		_map[pos.x, pos.y].Entity = null; 
	}

    // Querying
    // =====================================================
	public List<Vector2Int> GetOpenPoints() {
	    var openPoints = new List<Vector2Int>();
		for (int x = 0; x < Width; ++x) {
			for (int y = 0; y < Height; ++y) {
				if (IsOpen(x, y)) {
					openPoints.Add(new Vector2Int(x, y));
				}
			}
		}
		return openPoints;
	}

	public Entity GetEntityAt(int x, int y) {
		if (!IsValid(x, y)) return null;
		else return _map[x, y].Entity;
	}

	public bool HasEntity(int x, int y) {
		return IsValid(x, y) && _map[x, y].HasEntity();
	}

    public bool IsOpen(int x, int y) {
	    return IsValid(x, y) && _map[x, y].IsOpen();
    }

	public bool IsValid(int x, int y) {
       	return x >= 0 && y >= 0 && x < Width && y < Height;
	}

    // Internal Data Structures
    // =====================================================
    public class MapTile {
        public int Type;
        public Entity Entity;

		public bool HasEntity() {
			return Entity != null;
		}

        public bool IsOpen() {
            return Entity == null;
        }
    }
}
