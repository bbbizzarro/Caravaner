using System.Collections;
using System.Collections.Generic;
using System;
using Caravaner;
using Godot;

public interface IPathGrid {
    public List<Vector2Int> GetOpenNeighbors(int x, int y);
    public bool IsOpen(int x, int y);
}

public class PathFinder {

    #region Parameters 
    int maxSearchSize;
    #endregion

    #region MemberVariables
    IPathGrid grid;
    BinaryHeap<Vector2Int> pq;
    Dictionary<Vector2Int, TableEntry> table;
    #endregion

    public PathFinder(int maxSearchSize, IPathGrid grid) {
        this.grid = grid;
        this.maxSearchSize = maxSearchSize;
        table = new Dictionary<Vector2Int, TableEntry>();
        pq = new BinaryHeap<Vector2Int>(maxSearchSize, new NodeComparer(table));
	}

    #region Methods
    public List<Vector2Int> FindPath(Vector2Int start, Vector2Int end) {
        if (!grid.IsOpen(end.x, end.y)) return new List<Vector2Int>();
        pq.Clear();
        table.Clear();
        Add(start, start, 0);
        Vector2Int next = start;
        int iters = 0;
        float H;
        while (!pq.IsEmpty() && iters < maxSearchSize) {
            next = pq.Pop();
            foreach (var n in grid.GetOpenNeighbors(next.x, next.y)) {
                H = (end - n).Magnitude();
                Add(n, next, table[next].dist + 1 + H);
                if (n == end) {
                    return Trace(n);
				}
			}
            iters += 1;
		}
        return new List<Vector2Int>();
	}

    public List<Vector2> FindClosest(Vector2 startWorldPos) {
        return null;
	}
    #endregion


    #region Utils
    bool IsStart(Vector2Int node) {
        return node == table[node].prev;
	}

    List<Vector2Int> Trace(Vector2Int end) {
        Stack<Vector2Int> revPath = new Stack<Vector2Int>();
        Vector2Int next = end;
        while (!IsStart(next)) {
            revPath.Push(next);
            next = table[next].prev;
		}
        return new List<Vector2Int>(revPath);
	}

    void Add(Vector2Int node, Vector2Int prev, float dist) {
        if (table.ContainsKey(node)) {
            if (table[node].dist > dist) {
                table[node].dist = dist;
                pq.Update(node);
			}
            return;
		}
        else { 
			table.Add(node, new TableEntry(prev, dist));
			pq.Insert(node);
		}
	}
    #endregion

    #region Classes
    public class TableEntry {
        public Vector2Int prev;
        public float dist;
        public TableEntry(Vector2Int prev, float dist) {
            this.prev = prev;
            this.dist = dist;
		}
	}

    public class NodeComparer : IComparer<Vector2Int> {
        Dictionary<Vector2Int, TableEntry> table;
		public NodeComparer(Dictionary<Vector2Int, TableEntry> table) {
            this.table = table;
		}
        public int Compare(Vector2Int x, Vector2Int y) {
            if (!table.ContainsKey(x) || !table.ContainsKey(y)) {
                return 0;
			}
            if (table[x].dist > table[y].dist) {
                return 1;
			}
            return -1;
        }
    }
    #endregion
}
