using QFramework;
using UnityEngine;

public class StartGameCommand : AbstractCommand
{
    protected override void OnExecute()
    {
        this.SendEvent<GameStartEvent>();
        this.GetModel<GameModel>().ClikedNodesCount.Value = 0;
    }
}
