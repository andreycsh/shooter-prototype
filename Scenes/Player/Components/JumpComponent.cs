using Godot;

public partial class JumpComponent : Node
{
    [Export]
    private Player player;

    public override void _PhysicsProcess(double delta)
    {
        if (!player.IsOnFloor())
        {
            player.Velocity += player.GetGravity() * (float)delta;
        }

        if(Input.IsActionJustPressed("Jump") && player.IsOnFloor())
        {
            Vector3 playerVelocity = player.Velocity;
            playerVelocity.Y = player.JumpVelocity;
            player.Velocity = playerVelocity;
        }

        player.MoveAndSlide();
    }

    
    public bool CheckfallSpeed()
    {
        if (player.CurrentFallVelocity < player.FallVelocityThreshold)
        {
            player.CurrentFallVelocity = 0f;
            return true;
        } else
        {
            player.CurrentFallVelocity = 0f;
            return false;
        }
    }
}
