using Godot;

public partial class SimpleEnemy : BaseEnemy
{

    [Export]
    public float FollowSpeed = 3.0f;

    [Export]
    public NavigationAgent3D NavigationAgent;

    [Export]
    private HealthComponent healthComponent;

    [Export]
    public AnimationPlayer _AnimationPlayer;

    public Node3D Target;

    public override void _Ready()
    {
        base._Ready();

        Target = (Node3D)GetTree().GetFirstNodeInGroup("player");

        healthComponent.Connect(HealthComponent.SignalName.Died, Callable.From(Die));
        NavigationAgent.Connect(NavigationAgent3D.SignalName.VelocityComputed, Callable.From<Vector3>(OnVelocityComputed));

        if (_AnimationPlayer is not null)
        {
            _AnimationPlayer.Play("Fighting_Idle");
            _AnimationPlayer.Seek(GD.RandRange(0.0f, _AnimationPlayer.CurrentAnimationLength));
        }

    }

    public override void _PhysicsProcess(double delta)
    {
        if (!IsOnFloor())
        {
            Velocity = Velocity with { Y = Velocity.Y - 20.0f * (float)delta };

        }

        MoveAndSlide();
    }

    public override void OnTriggered()
    {
        GetNode("EnemyStateMachine").GetNode("FollowEnemyState").EmitSignal(NodeState.SignalName.TransitionState, nameof(EnemyStates.FollowEnemyState));
    }



    private void Die()
    {
        QueueFree();
    }

    private void OnVelocityComputed(Vector3 safeVelocity)
    {
        Velocity = Velocity with { X = safeVelocity.X, Z = safeVelocity.Z };
    }

    private void OnDetectionAreaBodyEntered(Node3D body)
    {
        if (body.IsInGroup("player"))
        {
            OnTriggered();
        }
    }

}
