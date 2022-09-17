using Godot;
using System;
using Caravaner;
using System.Collections.Generic;

public class Game : Node {

	[Export]
	public bool NEW_GAME = false;
	[Export]
	public string SAVE_FILE_PATH = "user://debug_save.save";
	WorldRenderer worldRenderer;
	World world;
	Player player;
	Simulator simulator;
	UI ui;
	WorldMap worldMap;
	WorldMapRenderer renderer;
	bool building = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		if (NEW_GAME) {
			NewGame();
		}
		else { 
			LoadGame();
		}
		Initialize();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta) {
		if (Input.IsActionJustPressed("ui_save")) {
			SaveGame();
		}
		simulator.Simulate(world, delta);
		ui.PlayerInformationToText(player);

		if (Input.IsActionJustPressed("ui_accept")) {
			worldMap.Seed(10);
		}

		if (Input.IsActionJustPressed("Build")) {
			building = !building;
		}

		if (building) {
			if (Input.IsActionJustPressed("Select1")) {
				var p = player.GlobalPosition;
				var pw = renderer.WorldToGrid(p);
				if (worldMap.IsOpen(pw.x, pw.y)) {
					worldMap.CreateEntity(new TentEntity("Tent", pw, worldMap));
				}
			}
			else if (Input.IsActionJustPressed("Select2")) {
				var p = player.GlobalPosition;
				var pw = renderer.WorldToGrid(p);
				if (worldMap.IsOpen(pw.x, pw.y)) {
					worldMap.CreateEntity(new CampFireEntity("CampFire", pw, worldMap));
				}
			}
		}
	}

	public void OnPlayerGridPositionChanged() {
		worldRenderer.DrawRadius(player.Position, 4);
	}

	private void NewGame() { 
		// Generate and initialize world
		var newWorldScene = (PackedScene)ResourceLoader.Load("res://World.tscn");
		var newWorld =(World)newWorldScene.Instance();
		AddChild(newWorld);
		newWorld.Generate();
		// Initialize player
		var newObjectScene = (PackedScene)ResourceLoader.Load("res://Player.tscn");
		var newObject = newObjectScene.Instance();
		AddChild(newObject);
	}

	public class WorldMapRenderer {
		WorldMap _worldMap;
		int _pixelsPerUnit;
		PackedScene viewScene = (PackedScene)ResourceLoader.Load("res://Scenes/View.tscn");
		Node _root;

		public WorldMapRenderer(WorldMap worldMap, int pixelsPerUnit, Node root) {
			_worldMap = worldMap;
			_worldMap.EntityCreatedEvent += InstantiateEntity;
			_pixelsPerUnit = pixelsPerUnit;
			_root = root;
		}

    	public AtlasTexture GetSprite(string name) {
    	    try {
    	        return ResourceLoader.Load<AtlasTexture>("res://Sprites/AtlasTextures/" + name + ".tres");
    	    }
    	    catch {
    	        return null;
    	    }
    	}

		public void InstantiateEntity(Entity entity) {
			var n = (Interactable)viewScene.Instance();
			_root.AddChild(n);
			n.Init(entity, this);
		}

		public Vector2 GridToWorld(Vector2Int pos) {
			return new Vector2(_pixelsPerUnit * pos.x, -_pixelsPerUnit * pos.y);
		}
    	public Vector2Int WorldToGrid(Vector2 pos) {
    	    return new Vector2Int(Mathf.RoundToInt(pos.x / _pixelsPerUnit),
    	                          Mathf.RoundToInt(Mathf.Abs(pos.y) / _pixelsPerUnit));
    	}
	}

	public delegate void HandleEntityCreation(Entity entity);

    public class WorldMap {

		public int Width {private set; get; }
		public int Height {private set; get; }
		Entity[,] _map;
		public event HandleEntityCreation EntityCreatedEvent;

		public WorldMap(int width, int height) {
			Width = width; Height = height;
			_map = new Entity[width, height];
		}

		public void Seed(int count) {
			var openPoints = new RandList<Vector2Int>(GetOpenPoints());
			for (int i = 0 ; i < count; ++i) {
				if (openPoints.Count > 0) {
					var point = openPoints.Pop();
					//CreateEntity(new Entity("Plant", new Vector2Int(point.x, point.y)));
					CreateEntity(new PlantEntity("Plant", new Vector2Int(point.x, point.y), this));
				}
			}
		}

		public void CreateEntity(Entity entity) {
			_map[entity.Position.x, entity.Position.y] = entity;
			entity.EntityDestroyedEvent += DestroyEntity;
			EntityCreatedEvent?.Invoke(entity);
		}

		public void DestroyEntity(Entity entity) {
			_map[entity.Position.x, entity.Position.y] = null;
		}

		public List<Vector2Int> GetOpenPoints() {
		    var openPoints = new List<Vector2Int>();
			for (int x = 0; x < Width; ++x) {
				for (int y = 0; y < Height; ++y) {
					if (_map[x,y] == null)
						openPoints.Add(new Vector2Int(x, y));
				}
			}
			return openPoints;
		}

        public bool IsOpen(int x, int y) {
			return IsValid(x, y) && _map[x, y] == null;
        }

		public bool IsValid(int x, int y) {
        	return x >= 0 && y >= 0 && x < Width && y < Height;
		}
    }

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

    private void Initialize() {
		// Initialize world.
		world = (World)GetNode("World");
		worldRenderer = (WorldRenderer)GetNode("WorldRenderer");
		worldRenderer.Initialize(world.GetWidth() * world.GetHeight(), 64, world);
		worldRenderer.DrawMap();

		// Initialize player.
		player = (Player)GetNode("Player");
		player.Connect("GridPositionChanged", this, "OnPlayerGridPositionChanged");
		player.Initialize(world);
		OnPlayerGridPositionChanged();

		int width = 10;
		int height = 10;
		worldMap = new WorldMap(width, height);
		renderer = new WorldMapRenderer(worldMap, 64, this);
		worldMap.Seed(10);
		player.GlobalPosition = new Vector2(64 * width / 2, -64 * height / 2);

		/*
		RegionGenerator rg = new RegionGenerator(10, 10, 8, 64);
		TileGenerator tg = new TileGenerator();
		GridMap gm = rg.Generate();
		Map regionMap = (Map)GetNode("RegionMap");
		RandList<Region> regions = new RandList<Region>(gm.GetOpenRegions());
		Region startingRegion = regions.Pop();
		tg.Generate(gm);
		startingRegion.visible = true;
		Vector2 startingPosition = gm.GridToWorld(startingRegion.center);
		player.GlobalPosition = startingPosition;
		regionMap.Initialize(gm, player);
		*/

		// Initialize simulator.
		simulator = new Simulator();

		// Initialize UI.
		ui = (UI)GetNode("UI");
		ui.Initialize(world, player);
	}

	private void LoadGame() {
		var saveGame = new File();
		if (!saveGame.FileExists(SAVE_FILE_PATH)) { 
			GD.Print(String.Format("No save file at '{0}'!", SAVE_FILE_PATH));
			return; // Error! We don't have a save to load.
		}

		// We need to revert the game state so we're not cloning objects during loading.
		// This will vary wildly depending on the needs of a project, so take care with
		// this step.
		// For our example, we will accomplish this by deleting saveable objects.
		var saveNodes = GetTree().GetNodesInGroup("Persist");
		foreach (Node saveNode in saveNodes)
			saveNode.QueueFree();

		// Load the file line by line and process that dictionary to restore the object
		// it represents.
		saveGame.Open(SAVE_FILE_PATH, File.ModeFlags.Read);

		while (saveGame.GetPosition() < saveGame.GetLen()) {
			// Get the saved dictionary from the next line in the save file
			var nodeData = new Godot.Collections.Dictionary<string, object>((Godot.Collections.Dictionary)JSON.Parse(saveGame.GetLine()).Result);

			// Ensure that key data is present.
			if (!nodeData.ContainsKey("Filename")) { 
				GD.Print("Data does not contain resource file name. Skipping.");
				continue;
			}
			if (!nodeData.ContainsKey("Parent")) { 
				GD.Print("Data does not contain resource parent node name. Skipping.");
				continue;
			}

			// Firstly, we need to create the object and add it to the tree and set its position.
			var newObjectScene = (PackedScene)ResourceLoader.Load(nodeData["Filename"].ToString());
			var newObject = (Node)newObjectScene.Instance();
			GetNode(nodeData["Parent"].ToString()).AddChild(newObject);

			// Check the node has a save function.
			if (!newObject.HasMethod("Load")) {
				GD.Print(String.Format("persistent node '{0}' is missing a Load() function, skipped", newObject.Name));
				continue;
			}

			newObject.Call("Load", nodeData);
		}
		saveGame.Close();
	}

	/*
	Default save file paths:

	Windows: %APPDATA%\Godot\app_userdata\[project_name]

	macOS: ~/Library/Application Support/Godot/app_userdata/[project_name]

	Linux: ~/.local/share/godot/app_userdata/[project_name]

	~/Library/Application\ Support/Godot/app_userdata/Caravaner
	*/

	private void SaveGame() {
		GD.Print("Saving game...");
		var saveGame = new File();
		saveGame.Open(SAVE_FILE_PATH, File.ModeFlags.Write);

		var saveNodes = GetTree().GetNodesInGroup("Persist");
		foreach (Node saveNode in saveNodes) {
			// Check the node is an instanced scene so it can be instanced again during load.
			if (saveNode.Filename.Empty()) {
				GD.Print(String.Format("persistent node '{0}' is not an instanced scene, skipped", saveNode.Name));
				continue;
			}

			// Check the node has a save function.
			if (!saveNode.HasMethod("Save")) {
				GD.Print(String.Format("persistent node '{0}' is missing a Save() function, skipped", saveNode.Name));
				continue;
			}

			// Call the node's save function.
			var nodeData = saveNode.Call("Save");

			// Store the save dictionary as a new line in the save file.
			saveGame.StoreLine(JSON.Print(nodeData));
			GD.Print(String.Format("persistent node '{0}' saved successfully.", saveNode.Name));

		}
		saveGame.Close();
		GD.Print("Save complete.");
	}

	private void _on_Player_Test() {
		GD.Print("Player test is called.");
	}
}


