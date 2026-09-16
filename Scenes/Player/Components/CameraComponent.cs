using Godot;

public partial class CameraComponent : Node
{
    [ExportCategory("Props & References")]
    [Export]
    private float lookSensitivity = 0.005f;

    [Export]
    private Player player;

    [Export]
    private Camera3D camera;

    [ExportCategory("Camera Effects")]
    [Export]
    private bool enableTilt = true;

    [Export]
    private bool enableFallKick = true;

    [Export]
    private bool enableDamageKick = true;

    [Export]
    private bool enableWeaponKick = true;

    [Export]
    private bool enableScreenShake = true;

    [Export]
    private bool enableHeadbob = true;

    [ExportCategory("Kick & Recoil Setting")]
    [ExportGroup("Run Tilt")]
    [Export]
    private float runPitch = 0.1f;  //degrees

    [Export]
    private float runRoll = 0.25f;  //degrees

    [Export]
    private float maxPitch = 1.0f;  //degrees

    [Export]
    private float maxRoll = 2.5f;   //degrees

    [ExportGroup("Camera Kick")]
    [ExportSubgroup("Fall Kick")]
    [Export]
    private float fallTime = 0.3f;

    [ExportSubgroup("DamageKick")]
    [Export]
    private float damageTime = 0.3f;

    [ExportSubgroup("WeaponKick")]
    [Export]
    private float weaponDecay = 0.5f;

    [ExportSubgroup("Headbob")]
    [Export(PropertyHint.Range, "0.0, 0.1, 0.01")]
    private float bobPitch = 0.05f;

    [Export(PropertyHint.Range, "0.0, 0.1, 0.01")]
    private float bobRoll = 0.025f;

    [Export(PropertyHint.Range, "0.0, 0.04, 0.01")]
    private float bobUp = 0.005f;

    [Export(PropertyHint.Range, "3.0, 8.0, 0.1")]
    private float bobFrequency = 6.0f;

    [ExportGroup("Step Smoothing")]
    [Export]
    private float stepSpeed = 8.0f;

    // Step smoothing params
    private float targetHeight = 0.0f;
    private bool stepSmoothing = false;
    private float offsetHeight;

    // Fall kick params
    private float fallValue = 0.0f;
    private float fallTimer = 0.0f;

    // Damage kick params
    private float damagePitch = 0f;
    private float damageRoll = 0f;
    private float damageTimer = 0f;

    // Weapon kick params
    private Vector3 weaponKickAngles = Vector3.Zero;

    // Screen shake params
    private Tween screenShakeTween;
    private const float MIN_SCREEN_SHAKE = 0.05f;
    private const float MAX_SCREEN_SHAKE = 0.5f;

    // Headbob params
    private float stepTimer = 0.0f;

    private const float DEFAULT_CAMERA_HEIGHT = 1.7f;

    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
        offsetHeight = DEFAULT_CAMERA_HEIGHT;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseMotion inputEvent && Input.MouseMode == Input.MouseModeEnum.Captured)
        {
            player.RotateY(-inputEvent.Relative.X * lookSensitivity);
            player.PlayerCameraController.RotateX(-inputEvent.Relative.Y * lookSensitivity);

            Vector3 cameraRotation = player.PlayerCameraController.Rotation;
            cameraRotation.X = Mathf.Clamp(cameraRotation.X, Mathf.DegToRad(-80), Mathf.DegToRad(80));
            player.PlayerCameraController.Rotation = cameraRotation;
        }

        if (Input.IsActionJustPressed("Escape"))
        {
            Input.MouseMode = Input.MouseModeEnum.Visible;
        }

        if (@event is InputEventMouseButton mouseButton && mouseButton.IsPressed())
        {
            Input.MouseMode = Input.MouseModeEnum.Captured;
        }
    }

    public override void _Process(double delta)
    {
        this.CalculateViewOffset(delta);

        if (stepSmoothing)
        {
            targetHeight = (float)Mathf.Lerp(targetHeight, 0.0, stepSpeed * delta);
            if (Mathf.Abs(targetHeight) < 0.01)
            {
                targetHeight = 0.0f;
                stepSmoothing = false;
            }

            Vector3 cameraPosition = player.PlayerCameraController.Position;
            cameraPosition.Y = offsetHeight + targetHeight;
            player.PlayerCameraController.Position = cameraPosition;
        }
    }

    public void SmoothStep(float heightChange)
    {
        targetHeight = heightChange;
        stepSmoothing = true;
    }

    public void UpdateCameraHeight(double delta, int direction)
    {
        if (offsetHeight >= 0.85 && offsetHeight <= DEFAULT_CAMERA_HEIGHT)
        {
            offsetHeight = (float)Mathf.Clamp(offsetHeight + player.SneakSpeed * direction * delta, 0.85, DEFAULT_CAMERA_HEIGHT);
        }
    }

    private void CalculateViewOffset(double delta)
    {
        if (player is null)
        {
            return;
        }

        fallTimer -= (float)delta;
        damageTimer -= (float)delta;

        Vector3 playerVelocity = player.Velocity;

        // headbob speed and timer value
        float speed = new Vector2(playerVelocity.X, playerVelocity.Z).Length();
        if (speed > 0.1 && player.IsOnFloor())
        {
            stepTimer += (float)delta * (speed / bobFrequency);
            stepTimer = (float)Mathf.PosMod(stepTimer, 1.0);
            //stepTimer = stepTimer % 1.0f;
        }
        else
        {
            stepTimer = 0.0f;
        }

        float bobSin = (float)Mathf.Sin(stepTimer * 2.0 * Mathf.Pi) * 0.5f;

        Vector3 angles = Vector3.Zero;
        Vector3 offset = Vector3.Zero;

        // Camera tilt
        if (enableTilt)
        {
            Vector3 forward = camera.GlobalTransform.Basis.Z;
            Vector3 right = camera.GlobalTransform.Basis.X;

            float forwardDot = playerVelocity.Dot(forward);
            float forwardTilt = Mathf.Clamp(forwardDot * Mathf.DegToRad(runPitch), Mathf.DegToRad(-maxPitch), Mathf.DegToRad(maxPitch));
            angles.X += forwardTilt;

            float rightDot = playerVelocity.Dot(right);
            float sideTilt = Mathf.Clamp(rightDot * Mathf.DegToRad(runRoll), Mathf.DegToRad(-maxRoll), Mathf.DegToRad(maxRoll));
            angles.Z -= sideTilt;
        }

        if (enableFallKick)
        {
            float fallRatio = Mathf.Max(0.0f, fallTimer / fallTime);
            float fallKickAmount = fallRatio * fallValue;
            angles.X -= fallKickAmount;
            offset.Y -= fallKickAmount;
        }

        if (enableDamageKick)
        {
            float damageRatio = Mathf.Max(0.0f, damageTimer / damageTime);
            angles.X += damageRatio * damagePitch;
            angles.Z += damageRatio * damageRoll;
        }

        if (enableWeaponKick)
        {
            weaponKickAngles = weaponKickAngles.MoveToward(Vector3.Zero, weaponDecay * (float)delta);
            angles += weaponKickAngles;
        }

        if (enableHeadbob)
        {
            float pitchDelta = bobSin * Mathf.DegToRad(bobPitch) * speed;
            angles.X -= pitchDelta;

            float rollDelta = bobSin * Mathf.DegToRad(bobRoll) * speed;
            angles.Z -= rollDelta;

            float bobHeight = bobSin * speed * bobUp;
            offset.Y += bobHeight;
        }

        camera.Position = offset;
        camera.Rotation = angles;
    }

    public void AddFallKick(float fallStrength)
    {
        fallValue = Mathf.DegToRad(fallStrength);
        fallTimer = fallTime;
    }

    public void AddDamageKick(float pitch, float roll, Vector3 source)
    {
        Vector3 forward = camera.GlobalTransform.Basis.Z;
        Vector3 right = camera.GlobalTransform.Basis.X;
        Vector3 direction = camera.GlobalPosition.DirectionTo(source);
        float forwardDot = direction.Dot(forward);
        float rightDot = direction.Dot(right);
        damagePitch = Mathf.DegToRad(pitch) * forwardDot;
        damageRoll = Mathf.DegToRad(roll) * rightDot;
        damageTimer = damageTime;
    }

    public void AddWeponKick(float pitch, float yaw, float roll)
    {
        weaponKickAngles.X += Mathf.DegToRad(pitch);
        weaponKickAngles.Y += Mathf.DegToRad((float)GD.RandRange(-yaw, yaw));
        weaponKickAngles.Z += Mathf.DegToRad((float)GD.RandRange(-roll, roll));
    }

    // Takes a value from 0.0 to 1.0 for screen shake strength and tweens that offset on both the horizontal and vertical offset of the camera over period of seconds
    public void AddScreenShake(float amount, float seconds)
    {
        screenShakeTween?.Kill();

        screenShakeTween = CreateTween();
        screenShakeTween.TweenMethod(Callable.From<float>((val) => UpdateScreenShake(val, amount)), 0.0, 1.0, seconds).SetEase(Tween.EaseType.Out);
    }

    private void UpdateScreenShake(float alpha, float amount)
    {
        amount = Mathf.Remap(amount, 0.0f, 1.0f, MIN_SCREEN_SHAKE, MAX_SCREEN_SHAKE);
        float currentShakeAmount = amount * (1.0f - alpha);
        camera.HOffset = (float)GD.RandRange(-currentShakeAmount, currentShakeAmount);
        camera.VOffset = (float)GD.RandRange(-currentShakeAmount, currentShakeAmount);
    }
}
