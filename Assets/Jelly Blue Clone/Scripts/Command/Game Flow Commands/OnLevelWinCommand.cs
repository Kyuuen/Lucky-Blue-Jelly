using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

public class OnLevelWinCommand  : AbstractCommand, ICommand
{
    public List<int> idBubblesLeft;
    private IGoldSystem _system;
    protected override void OnExecute()
    {
        _system = this.GetSystem<IGoldSystem>();
        _system.CalculateGold(idBubblesLeft);
        AudioManager.Instance.PlaySFXSound("winlevel1");
    }
}
