using Godot;

[GlobalClass]
public partial class HealthComponent : Node
{
    [Signal]
    public delegate void HealthChangedEventHandler(float newHealth, float maxHealth);

    [Signal]
    public delegate void DamageTakenEventHandler(float amount, Node3D source);

    [Signal]
    public delegate void DiedEventHandler();

    [Export]
    private float maxHealth = 100.0f;

    [Export]
    private bool startAtMax = true;

    private float currentHealth;

    private bool isAlive = true;

    public override void _Ready()
    {
        if (startAtMax)
        {
            currentHealth = maxHealth;
        }

        UniqueNameInOwner = true;
    }


    public void TakeDamage(float amount, Node3D source = null)
    {
        if (!isAlive)
        {
            return;
        }

        float actualDamage = Mathf.Max(0.0f, amount);
        currentHealth = Mathf.Max(0.0f, currentHealth - actualDamage);

        EmitSignal(SignalName.DamageTaken, actualDamage, source);
        EmitSignal(SignalName.HealthChanged, currentHealth, maxHealth);

        if (currentHealth <= 0.0f)
        {
            HandleDeath();
        }

    }

    public void Heal(float amount)
    {
        if (!isAlive)
        {
            return;
        }

        float actualHeal = Mathf.Max(0.0f, amount);
        currentHealth = Mathf.Min(maxHealth, currentHealth + actualHeal);

        EmitSignal(SignalName.HealthChanged, currentHealth, maxHealth);
    }

    private void HandleDeath()
    {
        if (!isAlive)
        {
            return;
        }

        isAlive = false;
        currentHealth = 0.0f;
        EmitSignal(SignalName.Died);
    }
}
