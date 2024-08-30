using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

public class ToMenuCommand : AbstractCommand
{
    IEventCenterSystem _eventCenterSystem;
    protected override void OnExecute()
    {
        _eventCenterSystem = this.GetSystem<IEventCenterSystem>();
        _eventCenterSystem.SendToMenuEvent();
    }
}
