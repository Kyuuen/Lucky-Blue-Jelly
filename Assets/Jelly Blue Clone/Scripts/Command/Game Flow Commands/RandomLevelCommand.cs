using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
public class RandomLevelCommand : AbstractCommand, ICommand
{   
    IEventCenterSystem _eventCenterSystem;

    public int _levelIndex;
    protected override void OnExecute()
    {
        _eventCenterSystem = this.GetSystem<IEventCenterSystem>();
        _eventCenterSystem.SendRandomLevelEvent(_levelIndex);
    }

}
