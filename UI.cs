using Godot;
using System;
using System.Collections.Generic;

public class UI : Node2D {
	private World world;
	private IContainer<int> player;
	private Database database;

	public void Initialize(World world, IContainer<int> player) {
		this.world = world;
		this.player = player;
		database = new Database();
		for (int i = 0; i < 20; ++i) {
			database.Add(new Item(i, String.Format("ITEM{0}", i)));
		}
	}

	public override void _Process(float delta) {
		//base._Process(delta);
		if (Input.IsActionJustPressed("interact")) {
			GD.Print(FormatItems(player.GetItems()));
		}
	}

	private string FormatItems(IEnumerable<int> items) {
		string sm = "[";
		foreach (int id in items) {
			sm += String.Format(" '{0}' ", database.Get(id));
		}
		sm += "]";
		return sm;
	}
}
