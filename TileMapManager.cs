using Godot;
using System;
using System.Collections.Generic;

public class TileMapManager : Node2D {
	Dictionary<MapType, TileMap> maps = new Dictionary<MapType, TileMap>();

	public override void _Ready() {
		// Get references to tilemaps
		maps[MapType.Floor] = (TileMap)GetNode("FloorMap");
		maps[MapType.Detail] = (TileMap)GetNode("DetailMap");
		maps[MapType.Static] = (TileMap)GetNode("StaticMap");
	}

	public TileMap Get(MapType mapType) {
		return maps[mapType];
	}

	public TileMap this[MapType mapType] {
		get => maps[mapType];
	}

}
public enum MapType { 
	Floor,
	Detail,
	Static
}
