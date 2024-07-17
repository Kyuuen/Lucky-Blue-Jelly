using UnityEngine;
using QFramework;

public class IncreaseCountCommand : AbstractCommand
{
    protected override void OnExecute()
    {
        this.GetModel<CounterAppModel>().Count.Value++;
    }
}
