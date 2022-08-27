using System;
using System.Collections.Generic;
using Godot;
using Caravaner;

public class Map : Node {
	TileMapManager maps;
	List<Vector2Int> directions = new List<Vector2Int>() {
			new Vector2Int(-1, 0),
			new Vector2Int(0, -1),
			new Vector2Int(1, 0),
			new Vector2Int(0, 1)
	};
	RandomNumberGenerator rng;
	HashSet<Vector2Int> visited;
	const float Scale = 64f;
	List<Region> regions = new List<Region>();

	private enum Dir { 
		Up = 0,
		Down = 1,
		Left = 2,
		Right = 3
	}

	public void Initialize() { 
		maps = (TileMapManager)GetNode("TileMapManager");
		rng = new RandomNumberGenerator();
		rng.Randomize();
		//PrintOctave(GenerateOctave(10), 10, 2);
		Vector2Int p1 = new Vector2Int(0, 0);
		Vector2Int p2 = new Vector2Int(-7, 0);
		Vector2Int p3 = new Vector2Int(-3, -7);
		PlaceScenes(GenerateFromPoint(p1.x, p1.y, rng.RandiRange(40, 140)));
		PlaceResources(GenerateFromPoint(p2.x, p2.y, rng.RandiRange(40, 140)));
		PlaceScenes(GenerateFromPoint(p3.x, p3.y, rng.RandiRange(40, 140)));
		DrawPath(p1, p2);
		DrawPath(p2, p3);
		//GenerateNodes(5, 10);
	}
	public void GenerateRegions(int n, int size) {
		// Create bounding box over current region

	}

	public void PlaceResources(Region region) { 
		var invalidPoints = new HashSet<Vector2Int>();
		Vector2Int cp = AllocPointRand(invalidPoints, region);
		PlaceScene(cp, "Exchange");
		cp = AllocPointRand(invalidPoints, region);
		PlaceScene(cp, "Mechanist");
		cp = AllocPointRand(invalidPoints, region);
		PlaceScene(cp, "ResourcePoint");
		cp = AllocPointRand(invalidPoints, region);
		PlaceScene(cp, "ResourcePoint");
		cp = AllocPointRand(invalidPoints, region);
		PlaceScene(cp, "CampSite");
	}

	public void PlaceScenes(Region region) {
		if (region == null || region.tiles == null) return;
		var invalidPoints = new HashSet<Vector2Int>();
		//PlaceScene(AllocPoint(invalidPoints, region), "Exchange");
		//PlaceScene(AllocPoint(invalidPoints, region), "Mechanist");
		//PlaceScene(AllocPoint(invalidPoints, region), "ResourcePoint");
		//PlaceScene(AllocPoint(invalidPoints, region), "ResourcePoint");
		//PlaceScene(AllocPoint(invalidPoints, region), "CampSite");
		Vector2Int cp = AllocPointRand(invalidPoints, region);
		PlaceScene(cp, "CityCenter");
		foreach (Vector2Int v in GetNeighbors(cp.x, cp.y)) { 
			if (!invalidPoints.Contains(v) && region.tiles.Contains(v) && maps[MapType.Static].GetCell(v.x, v.y)!= 5
				&& rng.Randf() > -1) {
				invalidPoints.Add(v);
				PlaceScene(v, "CityBlock");
			}
		}
	}

	public Vector2Int AllocPointRand(HashSet<Vector2Int> invalidPoints, Region region) { 
		Vector2Int point
			= GetRandomPoint(new List<Vector2Int>(region.tiles), invalidPoints);
		invalidPoints.Add(point);
		return point;
	}

	public void PlaceScene(Vector2Int position, string name) { 
		Vector2 v = new Vector2(position.x * Scale, position.y * Scale);
		Services.Instance.TileInstancer.Spawn(v, name);
	}

	public Vector2Int GetRandomPoint(List<Vector2Int> points, HashSet<Vector2Int> invalidPoints) {
		for (int i = 0; i < 100; ++i) {
			Vector2Int p = points[rng.RandiRange(0, points.Count - 1)];
			if (!invalidPoints.Contains(p) && maps[MapType.Static].GetCell(p.x, p.y)!= 5) return p;
		}
		GD.PrintErr("Could not get random point.");
		return Vector2Int.Zero;
	}

	public void GenerateNodes(int size, int gridSideLenth) {
		var nodes = new List<Vector2Int>();
		int length = size / 2 * gridSideLenth;
		for (int x = -length; x < length; x+= gridSideLenth) {
			for (int y = -length; y < length; y += gridSideLenth) {
				// Get random point in square.
				Vector2Int randomPoint = new Vector2Int(
					rng.RandiRange(x, x + gridSideLenth - 1),
					rng.RandiRange(y, y + gridSideLenth - 1)
				);
				nodes.Add(randomPoint);
			}
		}
	}

	public void DrawPath(Vector2Int start, Vector2Int finish) {
		Vector2Int direction = (finish - start).Normalized();
		int maxP = 0;

		while (start != finish && maxP < 100) {
			start += direction;
			if (!visited.Contains(start)) { 
			}
			maps[MapType.Floor].SetCell(start.x, start.y, 6);
			maps[MapType.Static].SetCell(start.x, start.y, -1);
			direction = (finish - start).Normalized();
			if (Mathf.Abs(direction.x) == 1 && Mathf.Abs(direction.y) == 1) {
				int x = Mathf.RoundToInt(rng.Randf());
				int y = (x == 1) ? 0 : 1;
				direction = new Vector2Int(Mathf.Sign(direction.x) * x,
										   Mathf.Sign(direction.y) * y);
			}
			maxP += 1;
		}
		maps[MapType.Floor].UpdateBitmaskRegion();
	}

	public Region GenerateFromPoint(int x, int y, int numberOfTiles) {
		if (visited == null)
			visited = new HashSet<Vector2Int>();
		if (visited.Contains(new Vector2Int(x, y))) {
			GD.PrintErr("Invalid generation point.");
			return null;
		}
		var neighbors = new List<Vector2Int>() { new Vector2Int(x, y) };
		Region region = new Region() {};
		region.position = new Vector2Int(x, y);
		HashSet<Vector2Int> regionTiles = new HashSet<Vector2Int>();
		for (int i = 0; i < numberOfTiles; ++i) {
			int candidateIndex = rng.RandiRange(0, neighbors.Count - 1);
			Vector2Int candidate = neighbors[candidateIndex];
			neighbors.RemoveAt(candidateIndex);
			visited.Add(candidate);
			maps[MapType.Floor].SetCell(candidate.x, candidate.y, 4);
			//SetActiveTile(candidate.x, candidate.y);
			regionTiles.Add(candidate);
			foreach (Vector2Int neighbor in GetNeighbors(candidate.x, candidate.y)) { 
				if (!visited.Contains(neighbor)) {
					neighbors.Add(neighbor);
					//tileMap.SetCell(neighbor.x, neighbor.y, 5);
				}
			}
		}
		// Fill in holes
		foreach (Vector2Int candidate in neighbors) {
			int filledCount = 0;
			foreach (Vector2Int neighbor in GetNeighbors(candidate.x, candidate.y)) {
				filledCount += 1;
			}
			if (filledCount >= 3) {
				regionTiles.Add(candidate);
				visited.Add(candidate);
				maps[MapType.Floor].SetCell(candidate.x, candidate.y, 4);
				//maps[MapType.Static].SetCell(candidate.x, candidate.y, 5);
			}

		}
		// Draw border
		foreach (Vector2Int r in regionTiles) {
			int ptwo = 0;
			int total = 0;
			foreach (Vector2Int neighbor in GetNeighbors(r.x, r.y)) {
				if (!regionTiles.Contains(neighbor)) {
					maps[MapType.Static].SetCell(r.x, r.y, 5);
				}
				if (neighbor - r == new Vector2Int(0, 1) && !visited.Contains(neighbor)) {
					maps[MapType.Floor].SetCell(neighbor.x, neighbor.y, 7);
				}
				total += ((regionTiles.Contains(neighbor)) ? 1 : 0) *
					Mathf.RoundToInt(Mathf.Pow(2, ptwo));
				ptwo += 1;
			}
			if (total == 15) continue;
			maps[MapType.Detail].SetCell(r.x, r.y, total);
		}
		//foreach (Vector2Int neighbor in neighbors) {
		//    if (!visited.Contains(neighbor)) {
		//        tileMap.SetCell(neighbor.x, neighbor.y, 2);
		//	}
		//}
		region.tiles = regionTiles;
		return region;
	}

	public List<Vector2Int> GetNeighbors(int x, int y) {
		var neighbors = new List<Vector2Int>();
		foreach (Vector2Int dir in directions) {
			neighbors.Add(new Vector2Int(x + dir.x, y + dir.y));
		}
		return neighbors;

	}

	public bool[,] GenerateOctave(int sideLength) {
		RandomNumberGenerator rng = new RandomNumberGenerator();
		rng.Randomize();
		bool[,] octave = new bool[sideLength, sideLength];
		for (int x = 0; x < sideLength; ++x) { 
			for (int y= 0; y < sideLength; ++y) {
				octave[x, y] = rng.Randf() > 0.5f;
			}
		}
		return octave;
	}
}
