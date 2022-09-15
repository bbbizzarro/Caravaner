using Godot;
using System;
using Caravaner;

public class UIView : CanvasLayer {

    PlayerModel _playerModel;
    GameWorld _gameWorld;
    bool _initialized;
    Label _eventText;
    Label _inspectorText;
    Label _fpsText;
    WorldSim _worldSim;

    public void Init(PlayerModel playerModel, GameWorld gameWorld) {
        _playerModel = playerModel;
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
        _inspectorText.Text = GetInspectorText();
        _fpsText.Text = GetFPSText();
        _eventText.Text = GetEventText();
        if (Input.IsActionJustPressed("interact")) {
            var options = _worldSim.GetOptions(_gameWorld, _playerModel);
            if (options.Count > 0) {
                options[0].Execute();
            }
        }
    }

    private string GetEventText() {
        string sm = "";
        foreach (var option in _worldSim.GetOptions(_gameWorld, _playerModel)) {
            sm += option.ToString() + "\n";
        }
        return sm;
    }

    private string GetFPSText(){
        return String.Format("{0} FPS", Engine.GetFramesPerSecond());
    }

    private string GetInspectorText() {
        return String.Format("Pos: {0}\nTile: {1}", 
            _playerModel.Position, 
            _gameWorld.GetTerrainAt(_playerModel.Position.x, _playerModel.Position.y));
    }
}
