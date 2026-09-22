using Godot;
using Godot.Collections;

public partial class Projectile : Area3D
{
    private Vector3 _velocity;
    private float _damage;

    public override void _Ready()
    {
        Connect(SignalName.BodyEntered, Callable.From<Node3D>(OnBodyEntered));
        GetTree().CreateTimer(3.0f).Timeout += QueueFree;
        
    }

    public override void _PhysicsProcess(double delta)
    {
        //GlobalPosition += _velocity * (float)delta;
        PhysicsDirectSpaceState3D spaceState =  GetWorld3D().DirectSpaceState;
        Vector3 startPos = GlobalPosition;
        Vector3 endPos = GlobalPosition + _velocity * (float)delta;

        PhysicsRayQueryParameters3D query = PhysicsRayQueryParameters3D.Create(startPos, endPos);
        query.CollisionMask = 1;
        Dictionary result = spaceState.IntersectRay(query);

        if (result.Count > 0)
        {
            GlobalPosition = (Vector3)result["position"];
            OnBodyEntered((Node3D)result["collider"]);
            return;
        }

        GlobalPosition = endPos;
    }

    public void SetupProjectile(Vector3 velocity, float damage)
    {
        _velocity = velocity;
        _damage = damage;
    }

    private void OnBodyEntered(Node3D body)
    {
        GD.Print("Projectile hit: ", body.Name, " ", GlobalPosition);
        SpawnImpactMarcker(GlobalPosition);

        HealthComponent healthComponent = body.GetNodeOrNull<HealthComponent>("HealthComponent");

        if(healthComponent is not null)
        {
            healthComponent.TakeDamage(_damage, GetParent<Node3D>());
        }

        QueueFree();
    }

    private void SpawnImpactMarcker(Vector3 position)
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

        GetTree().CreateTimer(2.0f).Timeout += marker.QueueFree;
    }
}
