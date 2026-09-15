using Godot;

public partial class NodeState : Node
{
    [Signal]
    public delegate void TransitionStateEventHandler(string state);

    public virtual void OnEnter() {}

    public virtual void OnExit() {}

    public virtual void UpdateProcess(double delta)
    {
    }

    public virtual void UpdatePhysicsProcess(double delta)
    {
    }


}
