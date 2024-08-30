using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using Unity.VisualScripting;

public class VibrationEnableCommand : AbstractCommand, ICommand
{
    public bool vibrationIsEnable;
    IEventCenterSystem _eventCenterSystem;
    IGameSceneModel _gameSceneModel;
    protected override void OnExecute()
    {
        _eventCenterSystem = this.GetSystem<IEventCenterSystem>();
        _gameSceneModel = this.GetModel<IGameSceneModel>();
        _eventCenterSystem.SendVibrationEnableEvent(vibrationIsEnable);
    }
}
