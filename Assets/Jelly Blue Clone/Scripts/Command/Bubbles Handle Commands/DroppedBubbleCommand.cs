using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
public class DroppedBubbleCommand : AbstractCommand, ICommand
{
    private IGameSceneModel mModel;
    protected override void OnExecute()
    {
        mModel = this.GetModel<IGameSceneModel>();
        mModel.IsDropping = false;
        this.SendEvent<DroppedBubbleEvent>();
    }
}
