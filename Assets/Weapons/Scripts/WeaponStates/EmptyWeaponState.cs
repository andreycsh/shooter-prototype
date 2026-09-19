using Godot;
using System;

public partial class EmptyWeaponState : NodeState
{
    [Export]
    private Player player;

    public override void UpdatePhysicsProcess(double delta)
    {

        EmitSignal(SignalName.TransitionState, nameof(WeaponStates.IdleWeaponState));
    }

    public override void OnEnter()
    {
        //player.PlayerWeaponContropller.FireWeapon();
    }
}
