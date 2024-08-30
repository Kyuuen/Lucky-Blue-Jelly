using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

public class BoosterPopupActivateCommand : AbstractCommand, ICommand
{
    public int status;
    IBoosterSystem boosterSystem;
    protected override void OnExecute()
    {
        boosterSystem = this.GetSystem<IBoosterSystem>();
        boosterSystem.OnBoosterPopupActivate(status);
    }
}
