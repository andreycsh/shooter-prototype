using Godot;

public partial class FireWeaponState : NodeState
{
    [Export]
    private Player player;

    public override void UpdatePhysicsProcess(double delta)
    {
        if (player.PlayerWeaponContropller.CurrentAmmo < 1)
        {
            EmitSignal(SignalName.TransitionState, nameof(WeaponStates.EmptyWeaponState));
        }

        if(player.PlayerWeaponContropller.CurrentWeapon.IsAutomatic)
        {
            if (Input.IsActionPressed("Fire"))
            {
                if (player.PlayerWeaponContropller.CanFire())
                {
                    player.PlayerWeaponContropller.FireWeapon();
                    return;
                }
            }
        }

        EmitSignal(SignalName.TransitionState, nameof(WeaponStates.IdleWeaponState));
    }

    public override void OnEnter()
    {
        player.PlayerWeaponContropller.FireWeapon();
    }


}
