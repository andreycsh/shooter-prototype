using Godot;

public partial class Weapon : Resource
{
    [Export]
    public string WeaponName = "Pistol";
    
    [Export]
    public float Damage = 25.0f;

    [Export]
    public int MaxAmmo = 15;

    [Export]
    public float Range = 25.0f;

    [Export]
    public float ProjectileSpeed = 50.0f;

    [Export]
    public bool IsHitscan = true;

    [Export]
    public PackedScene WeaponModel;

    [Export]
    public PackedScene ProjectileScene;

    [Export]
    public Vector3 WeaponPosition = new Vector3(0.2f, -0.2f, -0.3f);
}
