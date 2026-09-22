using Godot;

public partial class IdleWeaponState : NodeState
{
    [Export]
    private Player player;

    public override void UpdateProcess(double delta)
    {
        if (player.PlayerWeaponContropller.CurrentWeapon == null)
            return;
            
        if (Input.IsActionJustPressed("Fire") && player.PlayerWeaponContropller.CurrentWeapon.Ammo > 0)
        {
            EmitSignal(SignalName.TransitionState, nameof(WeaponStates.FireWeaponState));
        }
    }

}
