using Godot;

public partial class CrouchingState : NodeState
{
    [Export]
    private Player player;

    [Export]
    private PostureComponent postureComponent;


    public override void UpdateProcess(double delta)
    {
        postureComponent.UpdateCameraHeight(delta, -1);

        GD.Print("colliding " + player.PlayerShapeCast.IsColliding());
        if (!Input.IsActionPressed("Sneaking") && player.IsOnFloor() && !player.PlayerShapeCast.IsColliding())
        {
            EmitSignal(NodeState.SignalName.TransitionState, nameof(PlayerNodeStates.StandingState));
        }
    }

    public override void OnEnter()
    {
        postureComponent.Crouch();
    }

}
