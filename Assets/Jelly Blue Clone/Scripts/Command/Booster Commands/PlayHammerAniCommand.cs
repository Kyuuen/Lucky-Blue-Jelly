using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
public class PlayHammerAniCommand : AbstractCommand, ICommand
{
    public int iD;
    public Transform bubble;

    IEventCenterSystem _eventCenterSystem;
    protected override void OnExecute()
    {
        _eventCenterSystem = this.GetSystem<IEventCenterSystem>();
        _eventCenterSystem.SendHammerAniEvent(bubble,iD);
    }
}
