using QFramework;
using UnityEngine;

public interface ICounterAppModel : IModel
{
    BindableProperty<int> Count { get; }
}
