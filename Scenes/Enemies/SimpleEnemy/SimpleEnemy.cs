using Godot;

public partial class SimpleEnemy : BaseEnemy
{

    [ExportCategory("Properties")]
    [Export]
    public float FollowSpeed = 3.0f;

    [Export]
    private float acceleration = 3.0f;

    [Export]
    private float decceleartion = 1.0f;

    [Export]
    public float MeeleeRange = 1.0f;

    [ExportCategory("References")]
    [Export]
    public NavigationAgent3D NavigationAgent;

    [Export]
    private HealthComponent healthComponent;

    [Export]
    public AnimationPlayer _AnimationPlayer;

    [Export]
    public AnimationTree _AnimationTree;

    public Node3D Target;

    public AnimationNodeStateMachinePlayback AnimationTreeState;

    public override async void _Ready()
    {
        base._Ready();

        Target = (Node3D)GetTree().GetFirstNodeInGroup("player");

        healthComponent.Connect(HealthComponent.SignalName.Died, Callable.From(Die));
        NavigationAgent.Connect(NavigationAgent3D.SignalName.VelocityComputed, Callable.From<Vector3>(OnVelocityComputed));

        AnimationTreeState = _AnimationTree.Get("parameters/playback").As<AnimationNodeStateMachinePlayback>();

        /*GetTree().ProcessFrame += () =>
        {
            _AnimationTree.Set("parameters/Idle/seek_request", GD.RandRange(0.0f, 1.0f));
        };*/
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        _AnimationTree.Set("parameters/Idle/seek_request", GD.RandRange(0.0f, 1.0f));

    }

    public override void _PhysicsProcess(double delta)
    {
        if (!IsOnFloor())
        {
            Velocity = Velocity with { Y = Velocity.Y - 20.0f * (float)delta };

        }

        MoveAndSlide();
        UpdateBlends();
    }

    public override void OnTriggered()
    {
        EmitFollowState();
    }



    private void Die()
    {
        QueueFree();
    }

    private void OnVelocityComputed(Vector3 safeVelocity)
    {
        Vector3 targetVelocity = new Vector3(safeVelocity.X, Velocity.Y, safeVelocity.Z);
        float accel = Velocity.Length() > 0.01f ? acceleration : decceleartion;
        Velocity = Velocity with
        {
            X = Mathf.MoveToward(Velocity.X, targetVelocity.X, accel * (float)GetPhysicsProcessDeltaTime()),
            Z = Mathf.MoveToward(Velocity.Z, targetVelocity.Z, accel * (float)GetPhysicsProcessDeltaTime())
        };
    }

    private void OnDetectionAreaBodyEntered(Node3D body)
    {
        if (body.IsInGroup("player"))
        {
            OnTriggered();
        }
    }

    private void UpdateBlends()
    {
        float moveAmount = Velocity.Length();
        moveAmount = (float)Mathf.Remap(moveAmount, 0.0, FollowSpeed, 0.0, 1.0);
        _AnimationTree.Set("parameters/Follow/IdleChaseBlend/blend_position", moveAmount);
    }

    private void EmitFollowState()
    {
        GetNode("EnemyStateMachine").GetNode("FollowEnemyState").EmitSignal(NodeState.SignalName.TransitionState, nameof(EnemyStates.FollowEnemyState));
    }

    private void EmitAttackState()
    {
        GetNode("EnemyStateMachine").GetNode("AttackEnemyState").EmitSignal(NodeState.SignalName.TransitionState, nameof(EnemyStates.AttackEnemyState));

    }

}
