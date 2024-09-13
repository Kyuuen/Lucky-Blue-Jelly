using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

public interface IEventCenterSystem : ISystem 
{
    public void SendPreGameStartEvent();
    public void SendOnGameStartEvent();
    public void SendRestartLevelEvent();
    public void SendPlayOnEvent();
    public void SendNextLevelEvent();
    public void SendPauseEvent(bool gameIsPaused);
    public void SendVibrationEnableEvent(bool vibrationIsEnable);
    public void SendToMenuEvent();
    public void SendBreakObstaclesEvent(List<int> iceIds, List<int> wireIds);
    public void SendHammerAniEvent(Transform bubble, int iD);
    public void SendStrawberryAniEvent(int iD);
    public void SendFirecrackerAniEvent(Vector2 mousePos);
    public void SendRandomLevelEvent(int levelIndex);
    public void SendInactivateEvent();
}

public class EventCenterSystem : AbstractSystem, IEventCenterSystem
{
    protected override void OnInit()
    {

    }
    
    public void SendPreGameStartEvent()
    {
        this.SendEvent<PreGameStartEvent>();
    }

    public void SendOnGameStartEvent() 
    {
        this.SendEvent<OnGameStartEvent>();
    }

    public void SendRestartLevelEvent()
    {
        this.SendEvent<RestartLevelEvent>();
    }

    public void SendPlayOnEvent()
    {
        this.SendEvent<PlayOnEvent>();
    }

    public void SendNextLevelEvent()
    {
        this.GetModel<IPlayerPrefModel>().NextLevel();
        this.SendEvent<GetNextLevelEvent>();
    }

    public void SendPauseEvent(bool gameIsPaused)
    {
        this.SendEvent(new GamePauseEvent
        {
            gameIsPaused = gameIsPaused
        });
    }

    public void SendVibrationEnableEvent(bool vibrationIsEnable)
    {
        this.SendEvent(new VibrationEnableEvent
        {
            vibrationIsEnable = vibrationIsEnable
        });
    }

    public void SendToMenuEvent()
    {
        this.SendEvent<ToMenuEvent>();
    }

    public void SendBreakObstaclesEvent(List<int> iceIds, List<int> wireIds)
    {
        this.SendEvent(new BreakObstaclesEvent
        {
            iceIds = iceIds,
            wireIds = wireIds
        });
    }

    public void SendHammerAniEvent(Transform bubble, int iD)
    {
        this.SendEvent(new PlayHammerAniEvent
        {
            _iD = iD,
            _bubble = bubble
        });
    }

    public void SendStrawberryAniEvent(int iD)
    {
        this.SendEvent(new PlayStrawberryAniEvent
        {
            _iD = iD
        });
    }

    public void SendFirecrackerAniEvent(Vector2 mousePos)
    {
        this.SendEvent(new PlayFirecrackerAniEvent
        {
            _mousePos = mousePos
        });
    }

    public void SendRandomLevelEvent(int levelIndex)
    {
        this.SendEvent(new RandomLevelEvent
        {
            _levelIndex = levelIndex
        });
    }
    public void SendInactivateEvent()
    {
        this.SendEvent<BoosterInactivateEvent>();
    }
}
