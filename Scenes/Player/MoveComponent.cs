using Godot;

public partial class MoveComponent : Node
{
    private Vector2 _inputDirection;
    private float _speed = 8f;

    [Export]
    private CharacterBody3D player;

    [Export]
    private Node3D head;

    [Export]
    private float sprintSpeed = 8f;

    [Export]
    private float walkSpeed = 4f;

    [Export]
    private float sneakSpeed = 2f;

    [Export]
    private float acceleration = 40f;

    [Export]
    private float airControl = 5f;

    [Export]
    private float airResitance = 2f;

    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsActionPressed("Walking"))
        {
            _speed = walkSpeed;
        } else if (Input.IsActionPressed("Sneaking"))
        {
            _speed = sneakSpeed;
        } else
        {
            _speed = sprintSpeed;
        }

        _inputDirection = Input.GetVector("MoveLeft", "MoveRight", "MoveForward", "MoveBack");
        Vector3 direction = (head.Transform.Basis * new Vector3(_inputDirection.X, 0, _inputDirection.Y)).Normalized();

        Vector3 targetVelocity = direction * _speed;
        Vector3 horisontalVelocity = new Vector3(player.Velocity.X, 0, player.Velocity.Z);

        if (player.IsOnFloor())
        {
            horisontalVelocity = horisontalVelocity.MoveToward(targetVelocity, acceleration * (float) delta);
            player.Velocity = new Vector3(horisontalVelocity.X, player.Velocity.Y, horisontalVelocity.Z);
        } 
        else
        {
            if (direction != Vector3.Zero)
            {
                horisontalVelocity = horisontalVelocity.MoveToward(targetVelocity, airControl * (float) delta);
            } 
            horisontalVelocity = horisontalVelocity.MoveToward(Vector3.Zero, airResitance * (float) delta);
            player.Velocity = new Vector3(horisontalVelocity.X, player.Velocity.Y, horisontalVelocity.Z);

        }
        player.MoveAndSlide();
    }
}
