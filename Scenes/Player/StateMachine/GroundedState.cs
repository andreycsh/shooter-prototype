using Godot;

public partial class GroundedState : NodeState
{
    [Export]
    private Player player;

    public override void UpdatePhysicsProcess(double delta)
    {
        if (!player.IsOnFloor())
        {
            EmitSignal(SignalName.TransitionState, nameof(PlayerNodeStates.AirbornState));
        }
    }

}
