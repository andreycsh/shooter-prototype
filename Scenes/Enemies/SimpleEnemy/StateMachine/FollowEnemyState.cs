using Godot;

public partial class FollowEnemyState : NodeState
{
    [Export]
    private SimpleEnemy enemy;

    public override void UpdatePhysicsProcess(double delta)
    {
        if (enemy.Target is null)
        {
            return;
        }

        if (enemy.AnimationTreeState.GetCurrentNode() != "Follow")
        {
            enemy.AnimationTreeState.Travel("Follow");
        }

        enemy.NavigationAgent.TargetPosition = enemy.Target.GlobalPosition;

        float distance = enemy.GlobalPosition.DistanceTo(enemy.Target.GlobalPosition);
        if (distance <= enemy.MeeleeRange)
        {
            EmitSignal(SignalName.TransitionState, nameof(EnemyStates.AttackEnemyState));
            //return;
        }

        if (enemy.NavigationAgent.IsNavigationFinished())
        {
            enemy.NavigationAgent.Velocity = Vector3.Zero;
            if (enemy._AnimationPlayer is not null && enemy._AnimationPlayer.CurrentAnimation != "Fighting_Idle")
                enemy._AnimationPlayer.Play("Fighting_Idle");
            return;
        }

        Vector3 nextPosition = enemy.NavigationAgent.GetNextPathPosition();
        Vector3 direction = (nextPosition - enemy.GlobalPosition).Normalized();

        enemy.NavigationAgent.Velocity = direction * enemy.FollowSpeed;

        if (direction.Length() > 0.1)
        {
            float targetRotation = Mathf.Atan2(direction.X, direction.Z);
            enemy.Rotation = enemy.Rotation with { Y = Mathf.LerpAngle(enemy.Rotation.Y, targetRotation, 5.0f * (float)delta) };
        }
    }



}
