using Godot;
using Caravaner;

public class Facing {
    public Vector2 Curr {private set; get; }
    Vector2Int _lastInput;

    public Facing() {
        Curr = new Vector2(0, 1);
    }

    public void Update(Vector2 direction) {
        if (ToVector2Int(direction) == Vector2Int.Zero) return;
        Curr = new Vector2(Mathf.Round(direction.x), Mathf.Round(direction.y)).Normalized();
        //Curr = GetCurr(direction);
        //_lastInput = ToVector2Int(direction);
    }

    private Vector2 GetCurr(Vector2 direction) {
        var dirInt = ToVector2Int(direction);
        if (dirInt.x != _lastInput.x && dirInt.x != 0) {
            return new Vector2(dirInt.x, 0);
        }
        else if (dirInt.y != _lastInput.y && dirInt.y != 0) {
            return new Vector2(0, dirInt.y);
        }
        else {
            return Curr;
        }
        
    }

    private Vector2 ToVector2(Vector2Int v) {
        return new Vector2(v.x, v.y);
    }

    private Vector2Int ToVector2Int(Vector2 v) {
        return new Vector2Int(Mathf.RoundToInt(v.x),
                              Mathf.RoundToInt(v.y));
    }
}