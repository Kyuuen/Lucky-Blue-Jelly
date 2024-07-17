using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterAppModel : AbstractModel, ICounterAppModel
{
    public BindableProperty<int> Count { get; } = new BindableProperty<int>();

    protected override void OnInit()
    {
        base.OnDeinit();
    }
}
