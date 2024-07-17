using QFramework;
using QFramework.PointGame;
using System;
using UnityEngine;

public interface ICountDownSystem : ISystem
{
    int Time { get; }

    void Tick();
}

public class CountDownSystem : AbstractSystem, ICountDownSystem
{
    private DateTime mGameStartTime { get; set; }
    public int Time => 7 - (int) (DateTime.Now - mGameStartTime).TotalSeconds;

    private bool mGameStarted = false;
    protected override void OnInit()
    {
        this.RegisterEvent<GameStartEvent>(e =>
        {
            mGameStarted = true;
            mGameStartTime = DateTime.Now;
        });
        this.RegisterEvent<GamePassEvent>(e =>
        {
            mGameStarted = false;
        });
    }
    public void Tick()
    {
        if (mGameStarted) 
        {
            if (DateTime.Now - mGameStartTime > TimeSpan.FromSeconds(7))
            {
                mGameStarted = false;
                this.SendEvent(new OnCountDownEvent()
                {
                    isGameEnd = true
                });
            }
        }
        
    }
}
