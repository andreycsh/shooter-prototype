using Godot;
using Godot.Collections;

public partial class StepHandlerComponent : Node
{
    [ExportCategory("References")]
    [Export]
    private Player player;

    [ExportCategory("Step Settings")]
    [Export]
    private float surfaceThreshold = 0.3f;

    [Export]
    private float stepHeight = 0.5f;

    private const float FEET_ADJUSTED_POSITION = 0.05f;

    private const float MIN_STEP_HEIGHT = 0.1f;

    private const float MIN_MOVEMENT_LENGTH = 0.1f;

    private const float MIN_DOT_VALUE = 0.5f;

    public void HandleStepClimbing()
    {
        for (int i = 0; i < player.GetSlideCollisionCount(); i++)
        {
            KinematicCollision3D collision = player.GetSlideCollision(i);
            if (IsVerticalSurface(collision))
            {
                float measureHeight = MeasureStepHeight(collision);
                if (measureHeight > MIN_STEP_HEIGHT && measureHeight <= stepHeight && IsValidStepDirection(collision))
                {
                    Vector3 playerPosition = player.GlobalPosition;
                    playerPosition.Y += measureHeight + 0.1f;
                    player.GlobalPosition = playerPosition;
                    player.Velocity = player.PreviousVelocity;
                    player.PlayerCameraComponent.SmoothStep(measureHeight);
                }
                break;
            }
        }
    }

    private bool CheckCollisionNormal(KinematicCollision3D collision)
    {
        Vector3 normal = collision.GetNormal();
        if (Mathf.Abs(normal.Y) > surfaceThreshold)
            return false;
        return true;
    }

    private bool IsVerticalSurface(KinematicCollision3D collision)
    {
        Vector3 normal = collision.GetNormal();
        if (Mathf.Abs(normal.Y) <= surfaceThreshold)
        {
            return true;
        }
        return CheckCollisionSurface(collision);
    }

    private bool CheckCollisionSurface(KinematicCollision3D collision)
    {
        PhysicsDirectSpaceState3D spaceState = player.GetWorld3D().DirectSpaceState;
        Vector3 collisionPoint = collision.GetPosition();

        Vector3 playerFeetPosition = GetPlayerFeetPosition();
        collisionPoint.Y = playerFeetPosition.Y;

        PhysicsRayQueryParameters3D query = PhysicsRayQueryParameters3D.Create(playerFeetPosition, collisionPoint);
        query.CollisionMask = player.CollisionMask;
        query.Exclude = [player.GetRid()];

        Dictionary result = spaceState.IntersectRay(query);
        if (result.Count > 0 && Mathf.Abs(((Vector3)result["normal"]).Y) <= surfaceThreshold)
        {
            return true;
        }

        return false;
    }

    private float MeasureStepHeight(KinematicCollision3D collision)
    {
        PhysicsDirectSpaceState3D spaceState = player.GetWorld3D().DirectSpaceState;
        Vector3 collisionPoint = collision.GetPosition();

        Vector3 playerFeetPosition = GetPlayerFeetPosition();
        float playerHeadPositionY = player.GlobalPosition.Y + (((CapsuleShape3D)player.StandingCollision.Shape).Height / 2);

        Vector3 rayStart = new Vector3(collisionPoint.X, playerHeadPositionY, collisionPoint.Z);
        Vector3 rayEnd = new Vector3(collisionPoint.X, playerFeetPosition.Y, collisionPoint.Z);

        PhysicsRayQueryParameters3D query = PhysicsRayQueryParameters3D.Create(rayStart, rayEnd);
        query.CollisionMask = player.CollisionMask;
        query.Exclude = [player.GetRid()];

        Dictionary result = spaceState.IntersectRay(query);
        if (result.Count > 0)
        {
            return ((Vector3)result["position"]).Y - playerFeetPosition.Y;
        }

        return 0.0f;
    }

    private bool IsValidStepDirection(KinematicCollision3D collision)
    {
        Vector3 collisionNormal = collision.GetNormal();
        Vector2 inputDirecion = player.InputDirection;
        Vector3 movementDirection = player.Transform.Basis * new Vector3(inputDirecion.X, 0, inputDirecion.Y);
        if (movementDirection.Length() > MIN_MOVEMENT_LENGTH)
        {
            movementDirection = movementDirection.Normalized();
            float dotProduct = movementDirection.Dot(-collisionNormal);
            return dotProduct > MIN_DOT_VALUE;
        }
        return false;
    }

    private Vector3 GetPlayerFeetPosition()
    {
        Vector3 feetPosition = player.GlobalPosition;
        feetPosition.Y += FEET_ADJUSTED_POSITION;
        return feetPosition;
    }
}
