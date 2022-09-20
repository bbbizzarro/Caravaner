using Godot;
public delegate void EventHandler();
public class Health {
    public int CurrentHealth {private set; get; }
    public event EventHandler OnHealthZero;
    int _maxHealth;

    public Health(int maxHealth) {
        _maxHealth = maxHealth;
        CurrentHealth = maxHealth;
    }

    public void SetMaxHealth() {
        CurrentHealth = _maxHealth;
    }

    public void Increment(int amount) {
        CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, _maxHealth);
        if (CurrentHealth <= 0) {
            OnHealthZero?.Invoke();
        }
    }
}