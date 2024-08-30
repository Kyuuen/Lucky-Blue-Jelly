using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

public class PlayStrawberryAniCommand : AbstractCommand, ICommand
{
    public int iD;

    IEventCenterSystem _eventCenterSystem;
    protected override void OnExecute()
    {
        _eventCenterSystem = this.GetSystem<IEventCenterSystem>();
        _eventCenterSystem.SendStrawberryAniEvent(iD);
    }
}
