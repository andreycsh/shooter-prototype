using Godot;

public partial class IdleWeaponState : NodeState
{
    [Export]
    private Player player;

    public override void UpdateProcess(double delta)
    {
        if (Input.IsActionPressed("Fire") && player.PlayerWeaponContropller.CurrentAmmo > 0)
        {
            EmitSignal(SignalName.TransitionState, nameof(WeaponStates.FireWeaponState));
        } 

        if (player.PlayerWeaponContropller.CurrentAmmo <= 0)
        {
            EmitSignal(SignalName.TransitionState, nameof(WeaponStates.EmptyWeaponState));
        }
    }

}
