using Godot;

public class PlayerData {
    public const float MaxEnergy = 100;
    public const float MinEnergy = 0;
    public const float TickLength = 1f;
    public event HandleFloatEvent OnEnergySet;


    private float _energy = MaxEnergy;
    public float Energy {
        get {
            return _energy;
        }
        set {
            _energy = Mathf.Clamp(value, MinEnergy, MaxEnergy);
            OnEnergySet?.Invoke(value);
        }
    }

    public float EnergyPercent {
        get {return Energy / MaxEnergy;}
    }

    public void SetEnergyByTicks(float delta) {
        Energy -= TickLength * delta;
    }

}

public delegate void HandleFloatEvent(float value);