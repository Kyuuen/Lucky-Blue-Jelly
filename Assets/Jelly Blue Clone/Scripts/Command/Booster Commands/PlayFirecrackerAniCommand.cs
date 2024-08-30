using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

public class PlayFirecrackerAniCommand : AbstractCommand, ICommand
{
    public Vector2 mousePos;
    private IEventCenterSystem _eventCenterSystem;
    protected override void OnExecute()
    {
        _eventCenterSystem = this.GetSystem<IEventCenterSystem>();
        _eventCenterSystem.SendFirecrackerAniEvent(mousePos);
    }
}
