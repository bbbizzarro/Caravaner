using Godot;
using System;

public class InputManager {
    bool worldInputIsActive;

    public void SetWorldInput(bool active) {
        worldInputIsActive = active;
	}

    public bool WorldInputIsActive() {
        return worldInputIsActive;
	}
}
