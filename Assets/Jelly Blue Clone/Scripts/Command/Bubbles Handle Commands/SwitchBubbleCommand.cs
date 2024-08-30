using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

public class SwitchBubbleCommand : AbstractCommand, ICommand
{
    protected override void OnExecute()
    {
        this.SendEvent<SwitchBubbleEvent>();
    }
}
