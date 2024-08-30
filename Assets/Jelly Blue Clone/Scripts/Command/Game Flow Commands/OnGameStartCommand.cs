using UnityEngine;
using QFramework;

public class OnGameStartCommand : AbstractCommand, ICommand
{
    public int move;
    private IGameSceneModel mModel;
    private IScoreSystem mScoreSystem;

    private IEventCenterSystem mEventCenterSystem;
    protected override void OnExecute()
    {
        mModel = this.GetModel<IGameSceneModel>();
        mScoreSystem = this.GetSystem<IScoreSystem>();
        mEventCenterSystem = this.GetSystem<IEventCenterSystem>();
        mScoreSystem.levelIsEnd = false;
        mModel.IsDropping = false;
        mModel.SetNewScore();
        mModel.SetMove(move);

        mEventCenterSystem.SendOnGameStartEvent();
    }
}
