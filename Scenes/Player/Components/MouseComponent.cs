using Godot;

public partial class MouseComponent : Node
{
    [Export]
    private float lookSensitivity = 0.005f;

    [Export]
    private Node3D CameraController;

    [Export]
    private Camera3D camera;

    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseMotion inputEvent && Input.MouseMode == Input.MouseModeEnum.Captured) 
        {
            CameraController.RotateY(-inputEvent.Relative.X * lookSensitivity);
            camera.RotateX(-inputEvent.Relative.Y * lookSensitivity);

            Vector3 cameraRotation = camera.Rotation;
            cameraRotation.X = Mathf.Clamp(camera.Rotation.X, Mathf.DegToRad(-80), Mathf.DegToRad(80));
            camera.Rotation = cameraRotation;
        }

        if(Input.IsActionJustPressed("Escape"))
        {
            Input.MouseMode = Input.MouseModeEnum.Visible;
        }

        if(@event is InputEventMouseButton mouseButton && mouseButton.IsPressed())
        {
            Input.MouseMode = Input.MouseModeEnum.Captured;
        }
    }
}
