using Godot;

public partial class WeaponData : Resource
{
    [Export]
    public Weapon _Weapon;

    [Export]
    public bool Unlocked = false;

    [Export]
    public int Ammo = 0;
}
