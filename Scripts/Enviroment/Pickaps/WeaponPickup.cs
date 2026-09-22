using Godot;

public partial class WeaponPickup : BasePickap
{
    [Export]
    private int weaponSlot = 2;

    [Export]
    public Weapon WeaponResource;

    public override bool CanPickup(Player player)
    {
        WeaponData weaponData = WeaponManager.Instance.Weapons[weaponSlot];
        return !weaponData.Unlocked || weaponData.Ammo < WeaponResource.MaxAmmo;
    }

    public override void ApplyPickup(Player player)
    {
        WeaponData weaponData = WeaponManager.Instance.Weapons[weaponSlot];

        if (weaponData.Unlocked)
        {
            weaponData.Ammo = WeaponResource.MaxAmmo;
        }
        else
        {
            WeaponManager.Instance.UnlockWeapon(weaponSlot, WeaponResource);
            WeaponManager.Instance.SwitchToSlot(weaponSlot);
        }
    }


}
