using Godot;

public partial class BasePickap : Area3D
{
    [Export]
    public float RotationSpeed = 60.0f;

    [Export]
    public float FloatHeight = 0.1f;

    [Export]
    public float FloatSpeed = 2.0f;

    private float startY;

    private float time = 0.0f;

    public override void _Ready()
    {
        Connect(Area3D.SignalName.BodyEntered, Callable.From<Node3D>(OnPickup));
        startY = Position.Y;
    }

    public override void _Process(double delta)
    {
        time += (float)delta;
        this.Position = this.Position with {Y = startY + Mathf.Sin(time * FloatSpeed) * FloatHeight};

        this.RotateY(Mathf.DegToRad(RotationSpeed) * (float)delta);
    }

    public void OnPickup(Node3D body)
    {
        GD.Print("On body entered");
        GD.Print(body.Name);
        if(body is not Player player)
        {
            return;
        }

        if (CanPickup(player))
        {
            ApplyPickup(player);
            this.QueueFree();
        }


    }

    public virtual bool CanPickup(Player player)
    {
        return true;
    }

    public virtual void ApplyPickup(Player player)
    {
    
    }

}
