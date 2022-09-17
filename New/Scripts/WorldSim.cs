using System.Collections.Generic;
using Godot;

public class WorldSim {

    public List<Option> GetOptions(GameWorld world, PlayerModel playerModel) {
        List<Option> options = new List<Option>();
        return options;
    }
}

public class Option {
    public string Name {private set; get; }

    public Option(string name) {
        Name = name;
    }

    public override string ToString() {
        return Name;
    }

    public virtual void Execute() {
    }
}