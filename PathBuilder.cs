using System.Collections.Generic;
using Godot;
using Caravaner;

public class PathBuilder {
    private Dictionary<Vector2Int, PathEdge> ends;
    private PathEdge root;
    private List<PathEdge> edges;

    public PathBuilder() {
        ends = new Dictionary<Vector2Int, PathEdge>();
    }

    public IEnumerable<(Vector2Int, Vector2Int)> BuildPath() {
        var path = new List<(Vector2Int, Vector2Int)>();
        foreach (var edge in ends.Values) {
            if (ends.ContainsKey(edge.From)) {
                ends[edge.From].Next = edge;
            }
        }

        PathEdge curr = root;
        int count = 0;
        while (curr != null && count < ends.Count) {
            path.Add((curr.From, curr.To));
            curr = curr.Next;
            count += 1;
        }
        if (path.Count != ends.Count) {
            GD.PrintErr("Invalid path");
            return null;
        }
        return path;
    }

    public void Add(Vector2Int from, Vector2Int to) {
        var p = new PathEdge(from, to);
        if (root == null) {
            root = p;
        }
        if (ends.ContainsKey(to)) {
            GD.Print(ends[to].From, ends[to].To, from, to);
        }
        ends.Add(to, p);
    }

    private class PathEdge {
        public Vector2Int From {private set; get; }
        public Vector2Int To {private set; get; }
        public PathEdge Next {set; get;}

        public PathEdge(Vector2Int from, Vector2Int to) {
            From = from; To = to;
        }
    }
}
