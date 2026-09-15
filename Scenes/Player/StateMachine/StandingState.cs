using Godot;

public partial class StandingState : NodeState
{
    [Export]
    private Player player;

    [Export]
    private PostureComponent postureComponent;

    public override void UpdateProcess(double delta)
    {
        postureComponent.UpdateCameraHeight(delta, 1);

        if (Input.IsActionPressed("Sneaking") && player.IsOnFloor() )
        {
            EmitSignal(NodeState.SignalName.TransitionState, nameof(PlayerNodeStates.CrouchingState));
        }
    }

    public override void OnEnter()
    {
        postureComponent.Stand();
    }
}
