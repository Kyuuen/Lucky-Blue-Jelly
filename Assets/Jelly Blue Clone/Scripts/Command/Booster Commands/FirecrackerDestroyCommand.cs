using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using Newtonsoft.Json;

public class FirecrackerDestroyCommand : AbstractCommand, ICommand
{
    public List<int> _bubbleIds = new List<int>();
    public List<int> _iceIds = new List<int>();

    IBoosterSystem _boosterSystem;
    protected override void OnExecute()
    {
        _boosterSystem = this.GetSystem<IBoosterSystem>();
        _boosterSystem.FirecrackerDestroy(_bubbleIds, _iceIds);
    }
}
