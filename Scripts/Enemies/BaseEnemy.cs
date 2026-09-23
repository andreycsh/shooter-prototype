using Godot;

public partial class BaseEnemy : CharacterBody3D
{
    [Export]
    private string[] enemyGroups = [];

    public override void _Ready()
    {
        foreach (string group in enemyGroups)
        {
            AddToGroup(group);
        }
    }

    public virtual void OnTriggered()
    {
        
    }

}
