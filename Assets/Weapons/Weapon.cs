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
    public float FireRate = 2.0f;

    [Export]
    public bool IsAutomatic = false;

    [Export]
    public float Range = 25.0f;

    [Export(PropertyHint.Range, "0, 100")]
    public int Accuracy = 100;

    [Export]
    public float ProjectileSpeed = 30.0f;

    [Export]
    public bool IsHitscan = true;

    [Export]
    public PackedScene WeaponModel;

    [Export]
    public PackedScene ProjectileScene;

    [Export]
    public int PelletCount = 1;

    [Export]
    public float SpreadAngle = 0.0f;

    [Export]
    public Vector3 WeaponPosition = new Vector3(0.2f, -0.2f, -0.3f);
}
