using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

public class BreakObstaclesCommand : AbstractCommand, ICommand
{
    public List<int> iceIds, wireIds;
    IEventCenterSystem _eventCenterSystem;
    protected override void OnExecute()
    {
        _eventCenterSystem = this.GetSystem<IEventCenterSystem>();
        _eventCenterSystem.SendBreakObstaclesEvent(iceIds, wireIds);
    }
}
