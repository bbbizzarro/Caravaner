using System;
using System.Collections.Generic;
using Godot;
public class StateMachineDB {
    private Dictionary<string, HashSet<TransitionData>> transitions;

    public StateMachineDB(string path) {
        transitions = new Dictionary<string, HashSet<TransitionData>>();
        var csv = new CSV<TransitionData>();
        foreach (TransitionData record in csv.LoadRecordsfromFile(path)) {
            if (!transitions.ContainsKey(record.groupName)) {
                transitions.Add(record.groupName, new HashSet<TransitionData>());
			}
            if (!transitions[record.groupName].Contains(record)) {
                transitions[record.groupName].Add(record);
			}
		}
    }

    public StateMachine BuildStateMachine(string groupName) {
        if (!transitions.ContainsKey(groupName)) {
            GD.PrintErr(String.Format("No such state machine {0}", groupName));
            return null;
		}
        return new StateMachine(transitions[groupName]);
	}
}

public class TransitionData : ISavable, IRecord<string> {
    [SerializeField] public string groupName;
    [SerializeField] public string fromState;
    [SerializeField] public string input;
    [SerializeField] public string output;
    [SerializeField] public string toState;
    [SerializeField] public int probability;
    [SerializeField] public int inputFactor;
    [SerializeField] public string skill;

    public string GetKey() {
        return String.Format("{0}-{1}-{2}", groupName, fromState, toState);
    }

    public void Load(Godot.Collections.Dictionary<string, object> data) {
        JSONUtils.Deserialize(this, data);
    }

    public Godot.Collections.Dictionary<string, object> Save() {
        return JSONUtils.Serialize(this);
    }
}
