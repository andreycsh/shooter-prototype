using Godot;
using System;

public partial class MoveState : NodeState
{
    [Export]
    private Player player;

    public override void _Process(double delta)
    {
        if (player.InputDirection == Vector2.Zero && player.Velocity.Length() == 0)
        {
            EmitSignal(SignalName.TransitionState, nameof(PlayerNodeStates.IdleState));
        }
        else if (player.InputDirection != Vector2.Zero && player.Velocity.Length() != 0 && Input.IsActionPressed("Walking") && !Input.IsActionPressed("Sneaking"))
        {
            EmitSignal(SignalName.TransitionState, nameof(PlayerNodeStates.WalkState));
        }
        else if (player.InputDirection != Vector2.Zero && player.Velocity.Length() != 0 && Input.IsActionPressed("Sneaking"))
        {
            EmitSignal(SignalName.TransitionState, nameof(PlayerNodeStates.SneakState));
        }
        else { player.Speed = player.SprintSpeed; }
    }
}
