using System;
using System.Collections.Generic;
using Godot;

public class CategoryTree {
	private const string RootName = "Root";
	private readonly Dictionary<string, Node> db;

	public CategoryTree(string filePath) {
		db = Load(filePath);
	}

	public bool InCategory(string candidate, string required) {
		// i.e. is CANDIDATE equal to or a parent of REQUIRED
		// Follow candidate's parents until you get to required or beyond root.
		if (!db.ContainsKey(required)) {
			//GD.PrintErr(String.Format("No such category {0} in tree!.", required));
			return false;
		}
		if (!db.ContainsKey(candidate)) {
			//GD.PrintErr(String.Format("No such category {0} in tree!.", candidate));
			return false;
		}
		Node curr = db[candidate];
		while (curr != null) {
			if (curr.name == required)
				return true;
			curr = curr.parent;
		}
		return false;
	}

	private Dictionary<string, Node> Load(string filePath) { 
		var file = new File();
		if (!file.FileExists(filePath)) { 
			GD.PrintErr(String.Format("No save file at '{0}'!", filePath));
			return null; // Error! We don't have a save to load.
		}
		file.Open(filePath, File.ModeFlags.Read);
		Dictionary<string, Node> nodes = new Dictionary<string, Node>();
		nodes.Add(RootName, new Node(RootName, null));
		while (file.GetPosition() < file.GetLen()) {
			string[] edge = file.GetLine().Split(",");
			if (edge.Length != 2) {
				GD.PrintErr("Invalid Category Tree file format!");
				return null;
			}
			if (!nodes.ContainsKey(edge[1])) {
				nodes.Add(edge[1], new Node(edge[1], nodes[edge[0]]));
			}
			nodes[edge[0]].AddChild(nodes[edge[1]]);
		}
		nodes["Root"].Print(1);
		file.Close();
		return nodes;
	}

	private class Node {
		public string name;
		public List<Node> children;
		public Node parent;
		public Node(string name, Node parent) {
			this.name = name;
			this.parent = parent;
			children = new List<Node>();
		}

		public void AddChild(Node node) {
			children.Add(node);
		}

		public void Print(int level) {
			string tabString = "";
			for (int i =0; i < level; ++i) {
				tabString += "   |";
			}
			GD.Print(tabString + name);
			foreach (Node child in children) {
				child.Print(level + 1);
			}
		}
	}
}
