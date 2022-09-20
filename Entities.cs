using Godot;
using Caravaner;

public delegate void HandleEntityChanged(Entity entity);

public class Entity {
	public string Name {private set; get; }
	public string Sprite {private set; get; }
	public Vector2Int Position {private set; get;}
	public event HandleEntityChanged EntityDestroyedEvent;

	protected WorldMap _worldMap;

	public Entity(string name, string sprite, Vector2Int position, WorldMap worldMap) {
		Name = name; Position = position; Sprite = sprite;
		_worldMap = worldMap;
	}

	public void Destroy() {
		EntityDestroyedEvent?.Invoke(this);
	}

	public virtual void Interact(Player player, PlayerData playerData) {
	}
}

public class TentEntity : Entity {
	public TentEntity(string name, Vector2Int position, WorldMap worldMap) 
		: base(name, "atex_001", position, worldMap) {}
	
	public override void Interact(Player player, PlayerData playerData) {
		playerData.Energy = PlayerData.MaxEnergy;
		_worldMap.Seed(10);
	}
}

public class CampFireEntity : Entity {
	public CampFireEntity(string name, Vector2Int position, WorldMap worldMap) 
		: base(name, "atex_002", position, worldMap) {}
	
	public override void Interact(Player player, PlayerData playerData) {
		if (player.Inventory.HasItem("Plant Matter", 10)) {
			player.Inventory.TryToRemove("Plant Matter", 10);
			player.Inventory.TryToAdd(new DebugItem("Food", 1), 1);
		}
	}
}

public class PlantEntity : Entity {
	int _energyCost = 10;

	public PlantEntity(string name, Vector2Int position, WorldMap worldMap) 
		: base(name, "atex_000", position, worldMap) {
	}

	public override void Interact(Player player, PlayerData playerData) {
       	if (playerData.Energy >= _energyCost) {
       	    Services.Instance.PlayerData.SetEnergyByTicks(_energyCost);
       	    player.Inventory.TryToAdd(new DebugItem("Plant Matter", 1), 1);
			Destroy();
       	}

	}
}