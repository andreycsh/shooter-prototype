using Godot;

public partial class MoveComponent : Node
{
    [Export]
    private Player player;

    [Export]
    private float acceleration = 40f;

    [Export]
    private float airControl = 5f;

    [Export]
    private float airResitance = 2f;

    public override void _PhysicsProcess(double delta)
    {
        player.InputDirection = Input.GetVector("MoveLeft", "MoveRight", "MoveForward", "MoveBack");
        Vector3 direction = (player.Transform.Basis * new Vector3(player.InputDirection.X, 0, player.InputDirection.Y)).Normalized();

        Vector3 targetVelocity = direction * player.Speed;
        Vector3 horisontalVelocity = new Vector3(player.Velocity.X, 0, player.Velocity.Z);

        if (player.IsOnFloor())
        {
            horisontalVelocity = horisontalVelocity.MoveToward(targetVelocity, acceleration * (float)delta);
            player.Velocity = new Vector3(horisontalVelocity.X, player.Velocity.Y, horisontalVelocity.Z);
        }
        else
        {
            if (direction != Vector3.Zero)
            {
                horisontalVelocity = horisontalVelocity.MoveToward(targetVelocity, airControl * (float)delta);
            }
            horisontalVelocity = horisontalVelocity.MoveToward(Vector3.Zero, airResitance * (float)delta);
            player.Velocity = new Vector3(horisontalVelocity.X, player.Velocity.Y, horisontalVelocity.Z);

        }
        
        player.MoveAndSlide();

        if (player.IsOnFloor())
            player.PlayerStepHandler.HandleStepClimbing();
        /*if (player.InputDirection.Length() > 0.1)
        {
            Vector3 stairWalkerRotation = player.StairWalker.Rotation;
            stairWalkerRotation.Y = (Mathf.Atan2(-player.InputDirection.X, -player.InputDirection.Y));
            player.StairWalker.Rotation = stairWalkerRotation;

            if(player.IsOnWall() && player.IsOnFloor())
            {
                float stepHeight = player.StairRaycast.GetCollisionPoint().Y - player.GlobalPosition.Y;
                Vector3 playerGlobalPosition = player.GlobalPosition;
                playerGlobalPosition.Y += stepHeight + 0.05f;
                player.GlobalPosition = playerGlobalPosition;
                player.Velocity = player.PreviousVelocity;
            }
        }*/
        
    }
}
