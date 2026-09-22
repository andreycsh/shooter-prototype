using Godot;

public partial class GameManager : Node
{
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("Escape"))
        {
            GetTree().Quit();
        } else if (@event.IsActionPressed("DevReload"))
        {
            GetTree().ReloadCurrentScene();
        }
    }

}
