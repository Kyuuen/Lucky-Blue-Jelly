using QFramework;
using UnityEngine;

public interface IGameModel : IModel
{
    BindableProperty<int> ClikedNodesCount { get; }
}

public class GameModel : AbstractModel
{
    public BindableProperty<int> ClikedNodesCount { get; } = new BindableProperty<int>(0);

    protected override void OnInit()
    {
        
    }
}
