using Godot;
using System;
using Caravaner;

public class Game : Node {

	[Export]
	public bool NEW_GAME = false;
	[Export]
	public string SAVE_FILE_PATH = "user://debug_save.save";
	WorldRenderer worldRenderer;
	World world;
	Player player;
	Simulator simulator;

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

	private void Initialize() {
		world = (World)GetNode("World");
		worldRenderer = (WorldRenderer)GetNode("WorldRenderer");
		worldRenderer.Initialize(world.GetWidth() * world.GetHeight(), 64, world);
		worldRenderer.DrawMap();

		player = (Player)GetNode("Player");
		player.Connect("GridPositionChanged", this, "OnPlayerGridPositionChanged");
		player.Initialize(world);

		simulator = new Simulator();

		OnPlayerGridPositionChanged();
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


