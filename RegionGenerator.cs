using System;
using System.Collections;
using System.Collections.Generic;
using Godot;
using Caravaner;
using System.Linq;

public class RegionGenerator {

	// 1d tile groups
	// [^^^,^^,^],[*,-,|],etc
	Dictionary<string, Dictionary<string, int>> probs;
	RandomNumberGenerator rng;

	public RegionGenerator() {
		rng = new RandomNumberGenerator();
		rng.Randomize();
	}


	public IEnumerable Generate(int minX, int minY, int maxX, int maxY, IEnumerable<string> possible) {
		var tiles = new Dictionary<Vector2Int, WorldTile>();
		List<string> possibleList = new List<string>(possible);
		for (int x = minX; x < maxX; ++x) { 
			for (int y = minY; y < maxY; ++y) {
				Vector2Int position = new Vector2Int(x, y);
				tiles.Add(position, new WorldTile(new Vector2Int(x, y), possibleList[rng.RandiRange(0, possibleList.Count - 1)]));
			}
		}
		return tiles.Values; 
	}

	private void BuildProbs(List<string> possible) {
		int c = possible.Count;
		int[,] table = new int[c, c];
	}
}

public struct WorldTile {
	public Vector2Int position;
	public string tileName;

	public WorldTile(Vector2Int position, string tileName) {
		this.position = position;
		this.tileName = tileName;
	}
}
