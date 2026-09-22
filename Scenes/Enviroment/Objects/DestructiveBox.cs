using Godot;

public partial class DestructiveBox : StaticBody3D
{
    
    public void OnDestruct()
    {
        QueueFree();
    }
}
