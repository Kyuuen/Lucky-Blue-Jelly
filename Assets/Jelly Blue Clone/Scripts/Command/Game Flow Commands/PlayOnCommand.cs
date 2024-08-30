using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

public class PlayOnCommand : AbstractCommand, ICommand
{
    IEventCenterSystem _eventCenterSystem;
    protected override void OnExecute()
    {
        _eventCenterSystem = this.GetSystem<IEventCenterSystem>();
        _eventCenterSystem.SendPlayOnEvent();
    }
}
