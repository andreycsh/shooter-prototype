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

        enemy.NavigationAgent.TargetPosition = enemy.Target.GlobalPosition;

        if (enemy.NavigationAgent.IsNavigationFinished())
        {
            enemy.NavigationAgent.Velocity = Vector3.Zero;
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
