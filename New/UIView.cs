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
    WorldSim _worldSim;
    Vector2Int _lastPlayerPosition;

    public void Init(GameWorld.Entity player, GameWorld gameWorld) {
        _player = player;
        _gameWorld = gameWorld;
        _inspectorText = (Label)GetNode("HBoxContainer/Inspector/Text");
        _fpsText = (Label)GetNode("Top/FPS/Text");
        _initialized = true;
        _eventText = (Label)GetNode("HBoxContainer/Events/Text");
        _worldSim = new WorldSim();
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
        _lastPlayerPosition = _player.Position;
        if (Input.IsActionJustPressed("interact")) {
            GameWorld.Prop prop = _gameWorld.GetTileAt(_player.Position.x, _player.Position.y)
                                            .GetTopProp();
            if (prop != null) {
                prop.Interact();
            }
        }
    }

    private string GetEventText() {
        string sm = "";
        foreach (var prop in _gameWorld.GetPropsAt(_player.Position.x, _player.Position.y)) {
            sm += prop.Preview() + "\n";
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
