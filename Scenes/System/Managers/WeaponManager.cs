using Godot;
using Godot.Collections;

[GlobalClass]
public partial class WeaponManager : Node
{

    public static WeaponManager Instance;

    [Export]
    public Dictionary<int, WeaponData> Weapons = [];

    [Export]
    private Player player;

    private int currentSlot = 1;

    public override void _Ready()
    {
        Instance = this;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        for (int i = 0; i < 10; i++)
        {
            if (@event.IsActionPressed(i.ToString()))
            {
                SwitchToSlot(i);
                break;
            }
        }
    }

    public void UnlockWeapon(int slot, Weapon weapon)
    {
        Weapons[slot].Unlocked = true;
        Weapons[slot].Ammo = weapon.MaxAmmo;
    }

    public void SwitchToSlot(int slot)
    {
        if (!Weapons.ContainsKey(slot))
        {
            return;
        }
        WeaponData weapon = Weapons[slot];

        if (weapon != null && weapon.Unlocked)
        {
            currentSlot = slot;
            player.PlayerWeaponContropller.SwitchWeapon(weapon);
        }
    }

    public void UseAmmo(int slot, int amount = 1)
    {
        if (Weapons.ContainsKey(slot))
        {
            Weapons[slot].Ammo = Mathf.Max(0, Weapons[slot].Ammo - amount);
        }
    }

    public int GetCurrentAmmo()
    {
        return Weapons[currentSlot].Ammo;
    }
}
