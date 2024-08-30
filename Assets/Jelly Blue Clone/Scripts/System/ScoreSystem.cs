using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using System;

public interface IScoreSystem : ISystem 
{
    public bool levelIsEnd {  get; set; }
    public void CheckScore(int targetScore);
    public void ChangeScore(int iD);
}

public class ScoreSystem : AbstractSystem, IScoreSystem
{
    enum ScoreEachSize
    {
        Size_3 = 1,
        Size_5 = 3,
        Size_7 = 5,
        Size_10 = 7
    }


    public bool levelIsEnd { get; set; } = false;

    private IGameSceneModel _gameSceneModel;
    protected override void OnInit()
    {
        _gameSceneModel = this.GetModel<IGameSceneModel>();

        
        this.RegisterEvent<DoneBubbleEvent>(e =>
        {
            ChangeScore(e.id);
        });
    }

    public void ChangeScore(int iD)
    {
        ScoreEachSize[] scores = (ScoreEachSize[])Enum.GetValues(typeof(ScoreEachSize));
        int score = (int)scores[_gameSceneModel.Bubbles[iD].Size];
        _gameSceneModel.Score.Value += score;
    }

    public void CheckScore(int targetScore)
    {
        if (levelIsEnd) return;
        if(_gameSceneModel.Score.Value >= targetScore)
        {
            levelIsEnd = true;
            this.SendEvent<OnLevelWinEvent>();
        }
    }
}
