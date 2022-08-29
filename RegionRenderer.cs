using Godot;
using System.Collections.Generic;
using Caravaner;

public class RegionRenderer : Node2D {
    private RegionGenerator rg;
    private Timer timer;
    public bool ShowConnections { private set; get; }
    public bool GenerateOn {private set; get; }
    private Dictionary<Region, Color> regionColors;
    private RandomNumberGenerator rng = new RandomNumberGenerator();

    [Export] public int RegionGridWidth = 10;
    [Export] public int RegionGridHeight = 10;
    [Export] public int RegionGridSize = 15;
    [Export] public int TileScale = 64;
    [Export] public float AnimationDelay = 0f;

    public override void _Ready() {
        rng.Randomize();
        string hashString = "CreepsInThisPettyPace";
        ulong h =LongHash.GetHashCodeInt64(hashString);
        rg = new RegionGenerator(RegionGridWidth, RegionGridHeight, 
                                 RegionGridSize, TileScale);
        timer = (Timer)GetNode("Timer");
		timer.Connect("timeout", this, nameof(AnimateBuildRegions));
        Reset();
    }

    private void Reset() {
		GD.Print("Randomizing!");
        if (AnimationDelay <= 0) {
            rg.Generate();
        }
        else {
		    rg.Initialize();
		    rg.Step();
        }

        regionColors = new Dictionary<Region, Color>();
        foreach (var r in rg.GetRegions()) {
		    regionColors.Add(r, new Color(rng.Randf(), rng.Randf(), rng.Randf()));
        }
    }

	public override void _Process(float delta) {
		if (Input.IsActionJustPressed("interact")) {
            Reset();
			Update();
		}
		if (Input.IsActionJustPressed("interact2")) {
			ShowConnections = !ShowConnections;
			Update();
		}
		if (Input.IsActionJustPressed("ui_select")) {
			GenerateOn = !GenerateOn;
			Update();
		}

        if (AnimationDelay <= 0) return;

		if (GenerateOn) {
            if (rg.Step()) {
                GenerateOn = false;
            }
			Update();
			timer.Start(AnimationDelay);
		}
		else {
			timer.Stop();
		}
	}

	public void AnimateBuildRegions() { 
		rg.Step();
		Update();
	}


	// DRAWING
	// ========================================================================= 
	public override void _Draw() {
		DrawGrid();
		foreach (Region r in rg.GetRegions()) {
            DrawRegion(r);
			DrawConnections(r);
			DrawBorders(r);
		}
		foreach (Region r in rg.GetRegions()) {
			DrawRoads(r);
        }
	}

	private void DrawGrid() {
		Color gray = new Color(0.5f, 0.5f, 0.5f);
		//for (int i = -1; i <= rg.Width; ++i) {
		//	DrawLine(rg.IndexToWorld(new Vector2Int(i, -1)),
		//			 rg.IndexToWorld(new Vector2Int(i, rg.Height)),
		//			 gray);
		//}
		//for (int i = -1; i <= rg.Height; ++i) {
		//	DrawLine(rg.IndexToWorld(new Vector2Int(-1, i)),
		//			 rg.IndexToWorld(new Vector2Int(rg.Width, i)),
		//			 gray);
		//}
	}

	private void DrawBorders(Region r) {
        if (!rg.adjacenciesSet) return;
		Color c = (r.type == 0) ? new Color(0, 0, 0) : regionColors[r];
		foreach (var t in r.tiles) {
			if (!rg.gridMap.IsOpen(t.x, t.y)) {
				Vector2 pos = rg.GridToWorld(t);
				DrawRect(new Rect2(new Vector2(pos.x - rg.WorldScale/2f, pos.y - rg.WorldScale/2f), 
								   new Vector2(rg.WorldScale, rg.WorldScale)), 
								   c * 0.5f);
			}
		}
	}

	private void DrawRoads(Region r) { 
		foreach (var road in r.roadPaths) {
			if (road.Count < 2) continue;
			for (int i = 0; i < road.Count - 1; ++i) {
				DrawLine(rg.GridToWorld(road[i]),
						 rg.GridToWorld(road[i + 1]),
						 new Color(0, 0, 0));
			}
		}
	}

	private void DrawRegion(Region r) {
		Color c = (r.type == 0) ? new Color(0, 0, 0) : regionColors[r];
		foreach (Vector2Int tile in r.tiles) { 
			Vector2 pos = rg.GridToWorld(tile);
				DrawRect(new Rect2(new Vector2(pos.x - rg.WorldScale/2f, pos.y - rg.WorldScale/2f), 
								   new Vector2(rg.WorldScale, rg.WorldScale)), 
								   c);
		}
	}

	private void DrawConnections(Region r) {
        if (!ShowConnections) return;
		Color white = new Color(1, 1, 1);
		Color black = new Color(0, 0, 0);
		Color gray = new Color(1f, 1f, 1f, 0.1f);
		foreach (Region adj in r.adjacent) {
			DrawLine(rg.GridToWorld(r.center),
					 rg.GridToWorld(adj.center),
					 (adj.type == 0 || r.type == 0) ? gray : white);

		}
	}
}
