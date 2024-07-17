using QFramework;
using UnityEngine;

public class TappingGame : Architecture<TappingGame>
{
    protected override void Init()
    {
        this.RegisterModel(new GameModel());
        this.RegisterSystem(new CountDownSystem());
    }

    protected override void ExecuteCommand(ICommand command)
    {
        base.ExecuteCommand(command);
    }
}
