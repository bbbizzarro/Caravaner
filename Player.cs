using Godot;
using System.Collections.Generic;
using System;
using Caravaner;

public class Player : KinematicBody2D, ISavable, IContainer<int> {

	[SerializeField, Export] private float speed = 90f;
	[SerializeField] private int scale = 64;
	[SerializeField] private List<int> items;

	private float speedModifier = 1f;

	[Export] public float testProperty = 0f;
	private Vector2 direction = Vector2.Zero;
	private Vector2Int lastPos = Vector2Int.Zero;
	//private readonly float MAX_FRAME_RATE = 60f;
	[Signal] delegate void GridPositionChanged();
	private event ContainerUpdated ContainerUpdated;
	World world;
	InventoryUI inventoryUI;
	PlayerDropPoint playerDropPoint;
	Animator animator;
	public Inventory Inventory {private set; get; }
	public Inventory BuildInventory {private set; get; }

	private float INTERACT_DIST = 80f;
	Timer timer;
	bool waiting = false;
	Sprite workingIndicator;

	public override void _Ready() {
		animator = (Animator)GetNode("EntityView");
		timer = (Timer)GetNode("Timer");
		timer.Connect("timeout", this, nameof(SetWaiting));
		workingIndicator = (Sprite)GetNode("WorkingSprite");
	}

	public void SetWaiting() {
		waiting = false;
		((Sprite)workingIndicator).Hide();
	}

	public void SimulateAnimation(float length) {
		waiting = true;
		((Sprite)workingIndicator).Show();
		timer.Start(length);
	}

	public void Initialize(World world) {
		this.world = world;
		items = new List<int>() { 0, 1, 2, 3, 4, 5 };
		Inventory = new Inventory(100);
		BuildInventory = new Inventory(100);
		inventoryUI = (InventoryUI)GetNode("IconContainer");
		playerDropPoint = (PlayerDropPoint)GetNode("PlayerDropPoint");
		inventoryUI.Enable(false);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta) {
		if (waiting) return;
		direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		if (DragObject.IsDragging()) speedModifier = 0.3f;
		else speedModifier = 1f;
		//MoveAndCollide(MAX_FRAME_RATE * delta * speed * direction);
		MoveAndSlide(speedModifier * speed * direction);
		if (direction.Length() > 0) {
			animator.Walk();
			Services.Instance.WorldState.AdvanceTimeByTicks(delta);
			Services.Instance.PlayerData.SetEnergyByTicks(delta);
		}
		else {
			animator.Stop();
		}

		if (Input.IsActionJustPressed("interact2")) {
			if (Interactable.active != null) {
				Interactable.active.Interact();
			}
		}

		TrackLocation();
		HandlePointerClick();
		// DEBUGGGGGG
		if (Input.IsActionJustPressed("inventory_toggle")) {
			if (inventoryUI.IsEnabled()) {
				inventoryUI.Enable(false);
				playerDropPoint.Enable(true);
			}
			else { 
				inventoryUI.Enable(true);
				playerDropPoint.Enable(false);
			}
		}
		// DEBUGGGGGG
	}

	private void HandlePointerClick() { 
		if (Input.IsActionJustPressed("ui_select")) {
			Vector2 mousePos = GetGlobalMousePosition();
			//if ((mousePos - Position).Length() > INTERACT_DIST) {
			//	return;
			//}
			Tile tile = world.GetTile(mousePos);
			if (tile != null) {
				//tile.type = 0;
				world.UpdateTile(mousePos);
			}
		}
	}

	private void TrackLocation() {
		Vector2Int pos = new Vector2Int(Mathf.RoundToInt(Position.x / scale), 
										Mathf.RoundToInt(Position.y / scale));
		if (pos != lastPos) {
			lastPos = pos;
			EmitSignal(nameof(GridPositionChanged));
		}
	}

	public Vector2Int GetGridPosition() {
		return lastPos;
	}

	public Godot.Collections.Dictionary<string, object> Save() {
		var data = JSONUtils.SerializeNode(this);
		data["PosX"] = Position.x;
		data["PosY"] = Position.y;
		data["items"] = items;
		return data;
	}

	public void Load(Godot.Collections.Dictionary<string, object> data) {
		JSONUtils.Deserialize(this, data);
		if (data.ContainsKey("PosX") && data.ContainsKey("PosY")) {
			Position = new Vector2((float)data["PosX"], (float)data["PosY"]);
		}
	}

	public IEnumerable<int> GetItems() {
		return items;
	}

	public int Remove(int item) {
		items.Remove(item);
		ContainerUpdated?.Invoke();
		return item;
	}

	public void Add(int item) {
		items.Add(item);
		ContainerUpdated?.Invoke();
	}

	public void SubscribeToUpdate(ContainerUpdated receiver) {
		ContainerUpdated += receiver;
	}
}

public class DebugItem : IItem {

	public string Name {private set; get; }
	public int Weight {private set; get; }

	public DebugItem(string name, int weight) {
		Name = name; Weight = weight;
	}

    public string GetID() {
		return Name;
    }

    public int GetWeight() {
		return Weight;
    }
}