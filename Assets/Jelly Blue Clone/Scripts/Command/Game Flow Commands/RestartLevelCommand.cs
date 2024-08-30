using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartLevelCommand : AbstractCommand, ICommand
{
    IEventCenterSystem _eventSystem;
    protected override void OnExecute()
    {
        _eventSystem = this.GetSystem<IEventCenterSystem>();
        _eventSystem.SendRestartLevelEvent();
    }
}
