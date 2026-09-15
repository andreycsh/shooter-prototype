using Godot;

public partial class AirbornState : NodeState
{
    [Export]
    private Player player;

    public override void UpdatePhysicsProcess(double delta)
    {
        if (player.IsOnFloor())
        {
            if (player.PlayerJumpComponent.CheckfallSpeed())
            {
                player.PlayerCameraComponent.AddFallKick(2.0f);
            }
            EmitSignal(SignalName.TransitionState, nameof(PlayerNodeStates.GroundedState));
        }

        player.CurrentFallVelocity = player.Velocity.Y;
    }
}

