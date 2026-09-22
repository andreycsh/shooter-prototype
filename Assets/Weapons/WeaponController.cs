using Godot;
using Godot.Collections;

public partial class WeaponController : Node
{
    [Export]
    private Player player;

    [Export]
    public WeaponData CurrentWeapon;

    [Export]
    public Node3D WeaponModelParent;

    [Export]
    public Node WeaponStateChart;

    private Node3D CurrentWeaponModel;

    private bool canFireNext = true;

    private float fireRateTimer = 0f;

    public override void _Ready()
    {
        if (CurrentWeapon != null)
        {
            SpawnWeaponModel();
        }
    }

    public override void _Process(double delta)
    {
        if (fireRateTimer > 0)
        {
            fireRateTimer -= (float)delta;

            if(fireRateTimer <=0)
            {
                canFireNext = true;
            }
        }
    }
    public bool CanFire()
    {
        return CurrentWeapon.Ammo > 0 && canFireNext;
    }


    public void SpawnWeaponModel()
    {
        CurrentWeaponModel?.QueueFree();

        if (CurrentWeapon._Weapon.WeaponModel != null)
        {
            CurrentWeaponModel = CurrentWeapon._Weapon.WeaponModel.Instantiate<Node3D>();
            WeaponModelParent.AddChild(CurrentWeaponModel);
            CurrentWeaponModel.Position = CurrentWeapon._Weapon.WeaponPosition;
        }
    }

    public void FireWeapon()
    {
        if (!CanFire())
            return;

        CurrentWeapon.Ammo -=1;

        GD.Print(CurrentWeapon.Ammo);

        canFireNext = false;
        fireRateTimer = 1.0f / CurrentWeapon._Weapon.FireRate;

        if (CurrentWeapon._Weapon.IsHitscan)
        {
            this.PerformHitscan();
        }
        else
        {
            this.SpawnProjectile();
        }
    }

    public void SwitchWeapon(WeaponData weaponData)
    {
        CurrentWeapon = weaponData;

        CurrentWeaponModel?.QueueFree();

        SpawnWeaponModel();
    }

    private void PerformHitscan()
    {
        PhysicsDirectSpaceState3D spaceState = player.PlayerCamera.GetWorld3D().DirectSpaceState;
        Vector3 from = player.PlayerCamera.GlobalPosition;
        

        float accuracySpread = (100 - CurrentWeapon._Weapon.Accuracy) / 1000.0f;

        for (int i = 0; i < CurrentWeapon._Weapon.PelletCount; i++)
        {
            Vector3 forward = -player.PlayerCamera.GlobalTransform.Basis.Z;

            float accuracyX = (float)GD.RandRange(-accuracySpread, accuracySpread);
            float accuracyY = (float)GD.RandRange(-accuracySpread, accuracySpread);
            Vector3 direction = forward + new Vector3(accuracyX, accuracyY, 0f) * player.PlayerCamera.GlobalTransform.Basis;

            if (CurrentWeapon._Weapon.PelletCount  > 1)
            {
                float spreadX = (float)GD.RandRange(-CurrentWeapon._Weapon.SpreadAngle, CurrentWeapon._Weapon.SpreadAngle);
                float spreadY = (float)GD.RandRange(-CurrentWeapon._Weapon.SpreadAngle, CurrentWeapon._Weapon.SpreadAngle);
                direction += new Vector3(spreadX, spreadY, 0f) * player.PlayerCamera.GlobalTransform.Basis;
            }

            Vector3 to = from + direction * CurrentWeapon._Weapon.Range;

            PhysicsRayQueryParameters3D query = PhysicsRayQueryParameters3D.Create(from, to);
            Dictionary result = spaceState.IntersectRay(query);

            if (result.Count > 0)
            {
                //GD.Print("Hit: ", result["collider"], " At: ", result["position"], "\n", result);
                SpawnImpactMarcker((Vector3)result["position"]);

                ApplyDamageToTarget((Node3D)result["collider"]);
            }
        }
    }

    private async void SpawnImpactMarcker(Vector3 position)
    {
        MeshInstance3D marker = new MeshInstance3D();
        BoxMesh box = new BoxMesh();
        box.Size = new Vector3(0.05f, 0.05f, 0.05f);
        marker.Mesh = box;

        StandardMaterial3D material = new StandardMaterial3D();
        material.AlbedoColor = Colors.Red;
        marker.SetSurfaceOverrideMaterial(0, material);

        GetTree().CurrentScene.AddChild(marker);
        marker.GlobalPosition = position;
        //GetTree().CreateTimer(2.0).Timeout += marker.QueueFree();
        await ToSignal(GetTree().CreateTimer(2.0f), SceneTreeTimer.SignalName.Timeout);
        marker.QueueFree();
    }

    private void SpawnProjectile()
    {
        Projectile projectile = CurrentWeapon._Weapon.ProjectileScene.Instantiate<Projectile>();
        GetTree().CurrentScene.AddChild(projectile);

        projectile.GlobalPosition = player.PlayerCamera.GlobalPosition;

        float accuracySpread = (100 - CurrentWeapon._Weapon.Accuracy) / 1000.0f;

        Vector3 forward = -player.PlayerCamera.GlobalTransform.Basis.Z;

        float accuracyX = (float)GD.RandRange(-accuracySpread, accuracySpread);
        float accuracyY = (float)GD.RandRange(-accuracySpread, accuracySpread);
        Vector3 direction = forward + new Vector3(accuracyX, accuracyY, 0f);

        Vector3 velocity = direction * CurrentWeapon._Weapon.ProjectileSpeed;
        projectile.LookAt(projectile.GlobalPosition + direction, Vector3.Up);

        projectile.SetupProjectile(velocity, CurrentWeapon._Weapon.Damage);
    }

    private void ApplyDamageToTarget(Node3D target)
    {
        HealthComponent healthComponent = target.GetNodeOrNull<HealthComponent>("HealthComponent");

        if(healthComponent is not null)
        {
            healthComponent.TakeDamage(CurrentWeapon._Weapon.Damage, GetOwner<Node3D>());
        }
    }
}
