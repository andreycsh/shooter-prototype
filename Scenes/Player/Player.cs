using Godot;

public partial class Player : CharacterBody3D
{
    [ExportCategory("Player movement properties")]
    [Export]
    public float SprintSpeed = 8f;

    [Export]
    public float WalkSpeed = 4f;

    [Export]
    public float SneakSpeed = 2f;

    [Export]
    public float JumpVelocity = 4.5f;

    [Export]
    public float FallVelocityThreshold = -4.5f;

    public float CurrentFallVelocity = 0f;

    public float Speed { get; set; } = 0f;

    public Vector2 InputDirection { get; set; }

    [ExportCategory("Player posture properties")]
    [Export]
    public CollisionShape3D StandingCollision;

    [Export]
    public CollisionShape3D CrouchingCollision;

    [Export]
    public ShapeCast3D PlayerShapeCast;

    [Export]
    public float CrouchOffset = 0.8f;

    [ExportCategory("References")]
    [Export]
    public Camera3D PlayerCamera;
    
    [Export]
    public CameraController PlayerCameraController;

    [Export]
    public CameraComponent PlayerCameraComponent;

    [Export]
    public InteractionRaycast PlayerInteractionRaycast;

    [Export]
    public JumpComponent PlayerJumpComponent;

    [Export]
    public StepHandlerComponent PlayerStepHandler;

    [Export]
    public WeaponController PlayerWeaponContropller;

    public Vector3 PreviousVelocity;

    public override void _PhysicsProcess(double delta)
    {
        PreviousVelocity = Velocity;
    }
}
