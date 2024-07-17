using QFramework;
using QFramework.PointGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickNodeCommand : AbstractCommand
{
    protected override void OnExecute()
    {
        this.GetModel<GameModel>().ClikedNodesCount.Value++;
        if(this.GetModel<GameModel>().ClikedNodesCount.Value == 6)
        {
            this.SendEvent<GamePassEvent>();
        }
    }
}
