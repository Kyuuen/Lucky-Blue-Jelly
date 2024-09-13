using UnityEngine;
using QFramework;

public class OnGameStartCommand : AbstractCommand, ICommand
{
    public int _move;
    private IGameSceneModel _gameModel;
    private IScoreSystem _scoreSystem;

    private IEventCenterSystem _eventCenterSystem;
    protected override void OnExecute()
    {
        _gameModel = this.GetModel<IGameSceneModel>();
        _scoreSystem = this.GetSystem<IScoreSystem>();
        _eventCenterSystem = this.GetSystem<IEventCenterSystem>();
        _scoreSystem.levelIsEnd = false;
        _gameModel.IsDropping = false;
        _gameModel.SetNewScore();
        _gameModel.SetMove(_move);

        _eventCenterSystem.SendOnGameStartEvent();
    }
}
