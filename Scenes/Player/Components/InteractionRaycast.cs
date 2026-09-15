using Godot;

public partial class InteractionRaycast : RayCast3D
{
    private Node3D currentObject;

    public override void _Process(double delta)
    {
        if (IsColliding())
        {
            Node3D collidingObject = (Node3D)GetCollider();
            
            if (collidingObject == currentObject)
            {
                return;
            } else
            {
                currentObject = collidingObject;
                GD.Print("Object: " + collidingObject.Name);
            }
        } else
        {
            currentObject = null;
        }
    }

}
