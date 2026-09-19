using Godot;
using Godot.Collections;

public partial class WeaponController : Node
{
    [Export]
    private Player player;

    [Export]
    public Weapon CurrentWeapon;

    [Export]
    public Node3D WeaponModelParent;

    [Export]
    public Node WeaponStateChart;

    private Node3D CurrentWeaponModel;

    public int CurrentAmmo = 0;

    public override void _Ready()
    {
        if (CurrentWeapon != null)
        {
            SpawnWeaponModel();
            CurrentAmmo = CurrentWeapon.MaxAmmo;
        }
    }

    public void SpawnWeaponModel()
    {
        CurrentWeaponModel?.QueueFree();

        if (CurrentWeapon.WeaponModel != null)
        {
            CurrentWeaponModel = CurrentWeapon.WeaponModel.Instantiate<Node3D>();
            WeaponModelParent.AddChild(CurrentWeaponModel);
            CurrentWeaponModel.Position = CurrentWeapon.WeaponPosition;
        }
    }

    public void FireWeapon()
    {
        if (CurrentAmmo < 1)
            return;

        //CurrentAmmo -= 1;

        GD.Print(CurrentAmmo);

        if (CurrentWeapon.IsHitscan)
        {
            this.PerformHitscan();
        }
        else
        {
            this.SpawnProjectile();
        }
    }

    private void PerformHitscan()
    {
        PhysicsDirectSpaceState3D spaceState = player.PlayerCamera.GetWorld3D().DirectSpaceState;
        Vector3 from = player.PlayerCamera.GlobalPosition;
        Vector3 forward = -player.PlayerCamera.GlobalTransform.Basis.Z;
        Vector3 to = from + forward * CurrentWeapon.Range;

        PhysicsRayQueryParameters3D query = PhysicsRayQueryParameters3D.Create(from, to);
        Dictionary result = spaceState.IntersectRay(query);

        if (result.Count > 0)
        {
            //GD.Print("Hit: ", result["collider"], " At: ", result["position"], "\n", result);
            SpawnImpactMarcker((Vector3)result["position"]);
        }
    }

    private async void SpawnImpactMarcker(Vector3 position)
    {
        MeshInstance3D marker = new MeshInstance3D();
        BoxMesh box = new BoxMesh();
        box.Size = new Vector3(0.1f, 0.1f, 0.1f);
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
        Projectile projectile = CurrentWeapon.ProjectileScene.Instantiate<Projectile>();
        GetTree().CurrentScene.AddChild(projectile);

        projectile.GlobalPosition = player.PlayerCamera.GlobalPosition;

        Vector3 forward = -player.PlayerCamera.GlobalTransform.Basis.Z;
        Vector3 velocity = forward * CurrentWeapon.ProjectileSpeed;
        projectile.LookAt(projectile.GlobalPosition  + forward, Vector3.Up);

        projectile.SetupProjectile(velocity, CurrentWeapon.Damage);
    }
}
