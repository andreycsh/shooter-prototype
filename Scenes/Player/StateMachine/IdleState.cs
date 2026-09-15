using Godot;

public partial class IdleState : NodeState
{
    [Export]
    private Player player;

    public override void _Process(double delta)
    {
        if (player.InputDirection != Vector2.Zero && player.IsOnFloor())
        {
            EmitSignal(SignalName.TransitionState, nameof(PlayerNodeStates.MoveState));
        }
    }
}
