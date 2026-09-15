using Godot;

public partial class JumpComponent : Node
{
    [Export]
    private Player player;

    [Export]
    private float jumpVelocity = 4.5f;

    public override void _PhysicsProcess(double delta)
    {
        if (!player.IsOnFloor())
        {
            player.Velocity += player.GetGravity() * (float)delta;
        }

        if(Input.IsActionJustPressed("Jump") && player.IsOnFloor())
        {
            Vector3 playerVelocity = player.Velocity;
            playerVelocity.Y = jumpVelocity;
            player.Velocity = playerVelocity;
        }

        player.MoveAndSlide();
    }
}
