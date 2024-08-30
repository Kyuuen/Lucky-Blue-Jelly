using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

public class OnJellyCollectCommand : AbstractCommand, ICommand
{
    public Vector3 jelliesPos;
    public int type;

    IUISystem uiSystem;
    protected override void OnExecute()
    {
        uiSystem = this.GetSystem<IUISystem>();
        uiSystem.OnBubbleDone(jelliesPos, type);
    }
}
