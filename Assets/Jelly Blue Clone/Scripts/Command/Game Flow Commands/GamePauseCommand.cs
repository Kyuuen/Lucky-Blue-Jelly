using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
public class GamePauseCommand : AbstractCommand, ICommand
{
    IEventCenterSystem _eventCenterSystem;
    public bool gameIsPaused;
    protected override void OnExecute()
    {
        _eventCenterSystem = this.GetSystem<IEventCenterSystem>();
        _eventCenterSystem.SendPauseEvent(gameIsPaused);
    }
}
