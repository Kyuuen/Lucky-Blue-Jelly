using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
public class DroppedBubbleCommand : AbstractCommand, ICommand
{
    private IGameSceneModel _gameSceneModel;
    protected override void OnExecute()
    {
        _gameSceneModel = this.GetModel<IGameSceneModel>();
        _gameSceneModel.IsDropping = false;
        this.SendEvent<DroppedBubbleEvent>();
    }
}
