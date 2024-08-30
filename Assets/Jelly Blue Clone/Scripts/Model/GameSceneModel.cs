using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

public interface IGameSceneModel : IModel
{
    public int BubbleIds { get; set; }
    List<BubbleInfo> Bubbles { get; set; }
    List<int> SpawnedColors { get; set; }
    bool IsDropping { get; set; }

    BindableProperty<int> Score { get; set; }
    BindableProperty<int> Move {  get; set; }

    public void ResetModel();
    public void SetNewScore();
    public void SetMove(int move);
}

public class GameSceneModel : AbstractModel, IGameSceneModel
{
    public int BubbleIds { get; set; } = 0;
    public List<BubbleInfo> Bubbles { get; set; } = new List<BubbleInfo>();
    public List<int> SpawnedColors { get; set; } = new List<int>();
    public bool IsDropping { get; set; }

    public BindableProperty<int> Score { get; set; } = new BindableProperty<int>();
    public BindableProperty<int> Move { get; set; } = new BindableProperty<int>();

    protected override void OnInit()
    {
        ResetModel();
    }

    public void ResetModel()
    {
        BubbleIds = 0;
        IsDropping = false;
        Bubbles.Clear();
        SpawnedColors.Clear();
        Score.Value = 0;
    }

    public void SetNewScore()
    {
        Score.Value = 0;
    }

    public void SetMove(int move)
    {
        Move.Value = move;
    }
}
