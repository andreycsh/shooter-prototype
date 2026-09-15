using Godot;

public partial class PostureComponent : Node
{

    [Export]
    private Player player;

    [Export]
    private float crouchSpeed = 4f;

    private const float DEFAULT_HEIGHT = 1.7f;


    public void Stand()
    {
        player.StandingCollision.Disabled = false;
        player.CrouchingCollision.Disabled = true;
    }

    public void Crouch()
    {
        player.StandingCollision.Disabled = true;
        player.CrouchingCollision.Disabled = false;
    }

    public void UpdateCameraHeight(double delta, int direction)
    {
        if (player.PlayerCameraController.Position.Y >= player.CrouchOffset && player.PlayerCameraController.Position.Y <= DEFAULT_HEIGHT)
        {
            Vector3 position = player.PlayerCameraController.Position;
            position.Y = (float)Mathf.Clamp(position.Y + (crouchSpeed * direction) * delta, player.CrouchOffset, DEFAULT_HEIGHT);
            player.PlayerCameraController.Position = position;
        }
    }
    
}
