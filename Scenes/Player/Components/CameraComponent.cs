using Godot;

public partial class CameraComponent : Node
{
    [ExportCategory("Props & References")]
    [Export]
    private float lookSensitivity = 0.005f;

    [Export]
    private Player player;

    [Export]
    private Camera3D camera;

    [ExportCategory("Camera Effects")]
    [Export]
    private bool enableTilt = true;
    private bool enableFallKick = true;

    [ExportCategory("Kick & Recoil Setting")]
    [ExportGroup("Run Tilt")]
    [Export]
    private float runPitch = 0.1f;  //degrees

    [Export]
    private float runRoll = 0.25f;  //degrees

    [Export]
    private float maxPitch = 1.0f;  //degrees

    [Export]
    private float maxRoll = 2.5f;   //degrees

    [ExportGroup("Camera Kick")]
    [ExportSubgroup("Fall Kick")]
    [Export]
    private float fallTime = 0.3f;

    private float fallValue = 0.0f;
    private float fallTImer = 0.0f;

    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseMotion inputEvent && Input.MouseMode == Input.MouseModeEnum.Captured)
        {
            player.RotateY(-inputEvent.Relative.X * lookSensitivity);
            player.PlayerCameraController.RotateX(-inputEvent.Relative.Y * lookSensitivity);

            Vector3 cameraRotation = player.PlayerCameraController.Rotation;
            cameraRotation.X = Mathf.Clamp(cameraRotation.X, Mathf.DegToRad(-80), Mathf.DegToRad(80));
            player.PlayerCameraController.Rotation = cameraRotation;
        }

        if (Input.IsActionJustPressed("Escape"))
        {
            Input.MouseMode = Input.MouseModeEnum.Visible;
        }

        if (@event is InputEventMouseButton mouseButton && mouseButton.IsPressed())
        {
            Input.MouseMode = Input.MouseModeEnum.Captured;
        }
    }

    public override void _Process(double delta)
    {
        this.CalculateViewOffset(delta);
    }

    private void CalculateViewOffset(double delta)
    {
        if (player is null)
        {
            return;
        }

        fallTImer -= (float)delta;

        Vector3 playerVelocity = player.Velocity;

        Vector3 angles = Vector3.Zero;
        Vector3 offset = Vector3.Zero;

        // Camera tilt
        if (enableTilt)
        {
            Vector3 forward = camera.GlobalTransform.Basis.Z;
            Vector3 right = camera.GlobalTransform.Basis.X;

            float forwardDot = playerVelocity.Dot(forward);
            float forwardTilt = Mathf.Clamp(forwardDot * Mathf.DegToRad(runPitch), Mathf.DegToRad(-maxPitch), Mathf.DegToRad(maxPitch));
            angles.X += forwardTilt;

            float rightDot = playerVelocity.Dot(right);
            float sideTilt = Mathf.Clamp(rightDot * Mathf.DegToRad(runRoll), Mathf.DegToRad(-maxRoll), Mathf.DegToRad(maxRoll));
            angles.Z -= sideTilt;
        }
        if (enableFallKick)
        {
            float fallRatio = Mathf.Max(0.0f, fallTImer/ fallTime);
            float fallKickAmount = fallRatio * fallValue;
            angles.X -= fallKickAmount;
            offset.Y -= fallKickAmount;
        }

        camera.Position = offset;
        camera.Rotation = angles;
    }

    public void AddFallKick(float fallStrength)
    {
        fallValue = Mathf.DegToRad(fallStrength);
        fallTImer = fallTime;
    }

}
