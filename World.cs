using Godot;
using System;
using System.Collections.Generic;

public class World : Node2D {
	List<Tile> tiles;
	private int height = 10;
	private int width = 10;
	private int scale = 64;

	PackedScene tileScene = (PackedScene)ResourceLoader.Load("res://Tile.tscn");

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		GenerateMap();
		DrawMap();
	}

	// Might be better to create dictionary of data to instantiate with.
	private void GenerateMap() { 
		tiles = new List<Tile>();
		for (int x = 0; x < width; ++x) { 
			for (int y = 0; y < height; ++y) {
				tiles.Add(new Tile(x * scale, y * scale));
			}
		}
	}

	private void DrawMap() {
		foreach (Tile tile in tiles) { 
			var newTileObject = (Node2D)tileScene.Instance();
			AddChild(newTileObject);
			//newTileObject.Name = Guid.NewGuid().ToString();
			//newTileObject.Set("Position", new Vector2(tile.xPos, tile.yPos));
			//newTileObject.Set("Position", new Vector2(500, 500));
			newTileObject.Position = new Vector2(tile.xPos, -tile.yPos);
			//GD.Print(String.Format("Initializing tile at ('{0}', '{1}').", tile.xPos, tile.yPos));
		}
	}
}

public class Tile {
	public readonly int xPos;
	public readonly int yPos;
	public Tile(int xPos, int yPos) {
		this.xPos = xPos;
		this.yPos = yPos;
	}
}
