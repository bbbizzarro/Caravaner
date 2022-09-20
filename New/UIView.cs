using Godot;
using System;
using Caravaner;

public class UIView : CanvasLayer {

    GameWorld.Entity _player;
    GameWorld _gameWorld;
    bool _initialized;
    Label _eventText;
    Label _inspectorText;
    Label _fpsText;
    Label _inventoryText;
    WorldSim _worldSim;
    Vector2Int _lastPlayerPosition;
    int activeOption = 0;

    public void Init(GameWorld.Entity player, GameWorld gameWorld) {
        _player = player;
        _gameWorld = gameWorld;
        _inspectorText = (Label)GetNode("HBoxContainer/Inspector/Text");
        _fpsText = (Label)GetNode("Top/FPS/Text");
        _initialized = true;
        _eventText = (Label)GetNode("HBoxContainer/Events/Text");
        _worldSim = new WorldSim();
        _inventoryText = (Label)GetNode("HBoxContainer/Inventory/Text");
    }

    public override void _Process(float delta) {
        base._Process(delta);
        if (!_initialized) return;
        //inspectorText.Text = _playerModel.Position.ToString();
        if (_lastPlayerPosition != _player.Position) {
        }
        _inspectorText.Text = GetInspectorText();
        _fpsText.Text = GetFPSText();
        _eventText.Text = GetEventText();
        _inventoryText.Text = GetInventoryText();
        _lastPlayerPosition = _player.Position;
        if (Input.IsActionJustPressed("interact")) {
            var tile = GetPlayerTile();
            if (tile != null) tile.Interact(_player, activeOption);
        }
        if (Input.IsActionJustPressed("Cycle")) {
            SetActiveOption(1);
        }
    }

    private string GetInventoryText() {
        string sm = "";
        foreach (var i in _player.Items.GetItemValues()) {
            sm += String.Format("[x{0} {1}]", 
                _player.Items.NumberOf(i),
                i);
        }
        return sm;
    }

    private GameWorld.Tile GetPlayerTile() {
        return _gameWorld.GetTileAt(_player.Position.x, _player.Position.y);
    }

    private void SetActiveOption(int add) {
        var mx = GetPlayerTile().Preview(_player).Count;
        activeOption = (activeOption + add) % mx;
    }

    private string GetEventText() {
        SetActiveOption(0);
        var playerTile = GetPlayerTile();
        string sm = "";
        if (playerTile != null) {
            var options = playerTile.Preview(_player);
            for (int i = 0; i < options.Count; ++i) {
                sm += String.Format("{0}({1}) {2}\n", (i == activeOption) ? "*" : "", 
                        (i + 1).ToString(), options[i]);
            }
        }
        return sm;
    }

    private string GetFPSText(){
        return String.Format("{0} FPS", Engine.GetFramesPerSecond());
    }

    private string GetInspectorText() {
        return String.Format("Pos: {0}\nTile: {1}", 
            _player.Position, 
            _gameWorld.GetTileAt(_player.Position.x, _player.Position.y));
    }
}
