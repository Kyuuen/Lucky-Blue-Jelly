using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

public class GameOverCommand : AbstractCommand, ICommand
{
    protected override void OnExecute()
    {
        this.SendEvent<GameOverEvent>();
    }
}
