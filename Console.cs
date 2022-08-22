using Godot;
using System;
using System.Collections.Generic;

public class Console : LineEdit {
	bool isVisible;
	Dictionary<string, Command> commands;

	public override void _Ready() {
		base._Ready();
		Connect("text_entered", this, nameof(OnTextEntered));
		commands = new Dictionary<string, Command>() {
			{"SpawnIcon" , new SpawnIcon()}
		};
		Hide();
		PauseMode = PauseModeEnum.Process;
	}

	public void ToggleVisible() {
		if (isVisible) {
			Hide();
			GetTree().Paused = false;
			isVisible = false;
		}
		else {
			Show();
			GetTree().Paused = true;
			isVisible = true;
			Clear();
			GrabFocus();
		}
	}

	public override void _Process(float delta) {
		if (Input.IsActionJustPressed("console_toggle")) {
			ToggleVisible();
		}
	}

	public void OnTextEntered(string text) {
		string[] args = text.Split(" ");
		if (args.Length < 1 || !commands.ContainsKey(args[0])) {
			GD.PrintErr("Invalid command.");
			return;
		}
		commands[args[0]].Execute(args);
		Clear();
	}

	private class SpawnIcon : Command {
		public override void Execute(params string[] args) {
			if (args.Length < 4) {
				GD.Print("Error invalid command");
				return;
			}
			string name = args[1].Replace("_", " ");
			int x; int y;
			Int32.TryParse(args[2], out x);
			Int32.TryParse(args[3], out y);
			Services.Instance.IconInstancer.Spawn(name, new Vector2(x, y));
		}
	}

	private abstract class Command {
		public abstract void Execute(params string[] args);
	}
}
