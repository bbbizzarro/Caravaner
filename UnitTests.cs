using Godot;
using System;
using System.Collections.Generic;
using Caravaner;

public class UnitTests : Node2D {
    List<UnitTest> unitTests = new List<UnitTest>() {
        new PathBuilderTests()
    };

    public override void _Ready() {
        GD.Print("Beginning Tests!");
        foreach (var t in unitTests) {
            t.Test();
        }
        GD.Print("Done!");
    }
}

public interface UnitTest {
    void Test();
}

public class PathBuilderTests : UnitTest {
    public void Test() {
        if (!BasicTest()) PrintErrorMessage(nameof(BasicTest));
    }

    private void PrintErrorMessage(string name) {
        GD.PrintErr(String.Format("{0}:{1}->FAILED", nameof(PathBuilderTests), name));
    }

    private bool BasicTest() {
        PathBuilder p = new PathBuilder();
        Vector2Int e0 = new Vector2Int(0, 0);
        Vector2Int e1 = new Vector2Int(1, 0);
        Vector2Int e2 = new Vector2Int(1, -1);
        Vector2Int e3 = new Vector2Int(0, -1);

        p.Add(e0, e1);
        p.Add(e2, e3);
        p.Add(e1, e2);
        p.Add(e3, e0);
        var path = new List<(Vector2Int, Vector2Int)>(p.BuildPath());
        try {
            return path[0] == (e0, e1) && path[1] == (e1, e2) && 
                   path[2] == (e2, e3) && path[3] == (e3, e0);
        }
        catch {
            return false;
        }
    }
}