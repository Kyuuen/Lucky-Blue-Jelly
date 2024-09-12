using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using Newtonsoft.Json;

public interface IBoosterSystem : ISystem
{
    public void OnBoosterActive(int boosterType);
    public void OnBoosterPopupActivate(int boosterType);
    public void FirecrackerDestroy(List<int> bubbleIds, List<int> iceIds);
}
public class BoosterSystem : AbstractSystem, IBoosterSystem
{
    private IGameSceneModel _gameSceneModel;
    private IScoreSystem _scoreSystem;
    
    protected override void OnInit()
    {
        _scoreSystem = this.GetSystem<IScoreSystem>();
        _gameSceneModel = this.GetModel<IGameSceneModel>();

        this.RegisterEvent<PlayStrawberryAniEvent>(e =>
        {
            BreakListBubble(e._iD);
        });
    }

    public void OnBoosterActive(int boosterType)
    {
        List<int> bubbleIds = new List<int>();
        if(boosterType == 0)
        {
            bubbleIds = BoosterType0();
        }else if(boosterType == 1)
        {
            bubbleIds = BoosterType1();
        }
        this.SendEvent(new BoosterActivateEvent
        {
            _boosterStatus = boosterType,
            _bubbleIDs = bubbleIds
        });
    }

    List<int> BoosterType0()
    {
        List<int> result = new List<int>();
        for (int i = 0; i < _gameSceneModel.Bubbles.Count - 2; i++)
        {
            result.Add(i);
        }
        return result;
    }

    List<int> BoosterType1()
    {
        List<int> result = new List<int>();
        for (int i = 0; i < _gameSceneModel.Bubbles.Count - 2; i++)
        {
            if (IsSingleColor(_gameSceneModel.Bubbles[i].jellyColor))
            {
                result.Add(i);
            }
        }
        return result;
    }

    bool IsSingleColor(List<int> list)
    {
        for (int i = 1; i < list.Count; i++)
        {
            if (list[i] != list[0])
            {
                return false;
            }
        }
        return true;
    }

    void BreakListBubble(int iD)
    {
        List<int> result = new List<int>();
        for(int i = 0; i < _gameSceneModel.Bubbles.Count - 2; i++)
        {
            if (_gameSceneModel.Bubbles[i].jellyColor.Count <= 0) { continue; }
            if (_gameSceneModel.Bubbles[i].jellyColor[0] == _gameSceneModel.Bubbles[iD].jellyColor[0] && IsSingleColor(_gameSceneModel.Bubbles[i].jellyColor))
            {
                result.Add(_gameSceneModel.Bubbles[i].ID);
                _scoreSystem.ChangeScore(iD);
            }

        }
        this.SendEvent(new StrawberryBreakEvent
        {
            _iDs = result
        });
    }

    public void OnBoosterPopupActivate(int boosterType)
    {
        this.SendEvent(new BoosterPopupActivateEvent
        {
            _status = boosterType
        });
    }

    public void FirecrackerDestroy(List<int> bubbleIds, List<int> iceIds)
    {
        if(bubbleIds != null)
        {
            foreach (int bubbleId in bubbleIds)
            {
                _scoreSystem.ChangeScore(bubbleId);
            }
        }
        this.SendEvent(new FirecrackerDestroyEvent
        {
            _bubbleIds = bubbleIds,
            _iceIds = iceIds
        });
    }
}
