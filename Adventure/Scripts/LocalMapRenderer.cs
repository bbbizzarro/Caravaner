using Godot;
using System;
using Caravaner;

public class LocalMapRenderer : Node2D {
    TileMap _fov;
    FOVRenderer _fovRenderer;
    TileMap _ground;
    GroundRenderer _groundRenderer;
    TileMap _occlusionMap;
    TileMapOccluder _occluder;
    LocalMap _localMap;
    int _pixelsPerUnit = 12;
    PlayerEntity _playerEntity;

    public override void _Ready() {
    }

    public override void _Process(float delta) {
        //_fovRenderer.Render();
    }

    public void UpdateOcclusionMap(){
        _occluder.Update();
    }


    public LocalMapRenderer Init(int pixelsPerUnit, PlayerEntity playerEntity, LocalMap localMap) {
        _pixelsPerUnit = pixelsPerUnit;
        _playerEntity = playerEntity;
        _localMap = localMap;
        _fov = (TileMap)GetNode("FOV");
        _fovRenderer = new FOVRenderer(_localMap, _fov, _pixelsPerUnit, _playerEntity);
        _ground = (TileMap)GetNode("Land");
        _groundRenderer = new GroundRenderer(_ground, _localMap, _pixelsPerUnit);
        _groundRenderer.Render();
        _occlusionMap = (TileMap)GetNode("Occluder");
        _occluder = new TileMapOccluder(_occlusionMap, _localMap, _pixelsPerUnit);
        return this;
    }

}
