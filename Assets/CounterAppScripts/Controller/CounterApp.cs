using QFramework;
using System.Runtime.InteropServices;
using UnityEngine;

public class CounterApp : Architecture<CounterApp>
{
    protected override void Init()
    {
        this.RegisterModel(new CounterAppModel());
    }

    protected override void ExecuteCommand(ICommand command)
    {
        Debug.Log("current count: " + this.GetModel<CounterAppModel>().Count.ToString());
        base.ExecuteCommand(command);
    }
}
