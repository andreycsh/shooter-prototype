using Godot;

public partial class AttackEnemyState : NodeState
{
    [Export]
    private SimpleEnemy enemy;

    public override void OnEnter()
    {
        Attack();
    }

    private async void Attack()
    {
        float distance = enemy.GlobalPosition.DistanceTo(enemy.Target.GlobalPosition);
        if (distance <= enemy.MeeleeRange)
        {
            enemy.Velocity = Vector3.Zero;
            enemy.NavigationAgent.Velocity = Vector3.Zero;
            
            if (enemy.Target is not null)
            {
                Vector3 direction = (enemy.Target.GlobalPosition - enemy.GlobalPosition).Normalized();
                float targetRotation = Mathf.Atan2(direction.X, direction.Z);

                enemy.Rotation = enemy.Rotation with { Y = targetRotation };
            }
            enemy.AnimationTreeState.Travel("Attack");

            await ToSignal(enemy._AnimationTree, AnimationTree.SignalName.AnimationFinished);
            enemy.AnimationTreeState.Travel("Idle");
            await ToSignal(GetTree().CreateTimer(0.5f), SceneTreeTimer.SignalName.Timeout);

        }
        EmitSignal(SignalName.TransitionState, nameof(EnemyStates.FollowEnemyState));
    }
}
