using Godot;
using System.Collections.Generic;

public delegate void InteractEventHandler();
public delegate void InteractObjectEventHandler(Interactor interactor);
public delegate void NodePackageEventHandler(Node node);

public class Interactor : Area2D {
    public event NodePackageEventHandler ReceiveEvent;
    public event InteractObjectEventHandler EnterEvent;
    public event InteractObjectEventHandler ExitEvent;
    public event InteractEventHandler FocusEvent;
    public event InteractEventHandler UnfocusEvent;

    public Interactor Current {private set; get;}
    protected List<Interactor> _inAreas = new List<Interactor>();

    public override void _Ready() {
        Connect("area_entered", this, nameof(OnInteractableEnter));
        Connect("area_exited", this, nameof(OnInteractableExit));
    }

    public void OnInteractableEnter(Area2D area) {
        Interactor last = Current;
        _inAreas.Add((Interactor)area);
        Current = GetNearest();
        if (last != Current) 
            OnNewEnter(Current);
    }

    public void OnInteractableExit(Area2D interactor) {
        Interactor last = Current;
        _inAreas.Remove((Interactor)interactor);
        Current = GetNearest();
        if (last != Current) {
            OnNewExit(last);
            if (Current != null)
                OnNewEnter(Current);
        }
    }

    protected virtual void OnNewEnter(Interactor current) {
        EnterEvent?.Invoke(current);
    }
    protected virtual void OnNewExit(Interactor last) {
        ExitEvent?.Invoke(last);
    }

    public void Focus() {
        FocusEvent?.Invoke();
    }

    public void Unfocus() {
        UnfocusEvent?.Invoke();
    }

    public void Send(Node node) {
        if (Current != null) {
            Current.Receive(node);
        }
    }

    public void Receive(Node node) {
        ReceiveEvent?.Invoke(node);
    }
    
    private Interactor GetNearest() {
        if (_inAreas.Count == 0) return null;
        else {
            Interactor nearest = _inAreas[0];
            float shortestLength = (nearest.GlobalPosition - GlobalPosition).Length();
            for (int i = 1; i < _inAreas.Count; ++i) {
                if ((_inAreas[i].GlobalPosition - GlobalPosition).Length() < shortestLength) {
                    nearest = _inAreas[i];
                }
            }
            return nearest;
        }
    }

    public bool IsConnectedTo(Interactor interactor) {
        return interactor.Current == this;
    }
}
