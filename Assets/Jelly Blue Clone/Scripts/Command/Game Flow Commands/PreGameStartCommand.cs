using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

public class PreGameStartCommand : AbstractCommand, ICommand
{
    IEventCenterSystem _eventSystem;
    IPlayerPrefModel _prefModel;
    protected override void OnExecute()
    {
        _eventSystem = this.GetSystem<IEventCenterSystem>();
        _prefModel = this.GetModel<IPlayerPrefModel>();
        _eventSystem.SendPreGameStartEvent();
    }
}
