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

    [ExportCategory("Player components")]
    [Export]
    public CameraController PlayerCameraController;

    [Export]
    public InteractionRaycast PlayerInteractionRaycast;

}
