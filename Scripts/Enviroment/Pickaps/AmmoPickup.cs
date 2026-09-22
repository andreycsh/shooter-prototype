using Godot;

public partial class AmmoPickup : BasePickap
{
    [Export]
    private int weaponSlot = 2;

    [Export]
    private int ammoAmount = 10;

    public override bool CanPickup(Player player)
    {
        WeaponData weaponData = WeaponManager.Instance.Weapons[weaponSlot];
        return weaponData.Unlocked && weaponData.Ammo < weaponData._Weapon.MaxAmmo;
    }

    public override void ApplyPickup(Player player)
    {
        WeaponData weaponData = WeaponManager.Instance.Weapons[weaponSlot];

        weaponData.Ammo += Mathf.Min(ammoAmount, weaponData._Weapon.MaxAmmo - weaponData.Ammo);
    }
}
