using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

public class BoosterInactivateCommand : AbstractCommand, ICommand
{
    IEventCenterSystem _eventCenterSystem;
    IPlayerPrefModel _playerPrefModel;
    public int _boosterType;
    public bool _isPopupOn;

    protected override void OnExecute()
    {
        _playerPrefModel = this.GetModel<IPlayerPrefModel>();
        _eventCenterSystem = this.GetSystem<IEventCenterSystem>();
        _eventCenterSystem.SendInactivateEvent();
        if (!_isPopupOn)
        {
            switch (_boosterType)
            {
                case 0:
                    {
                        if (_playerPrefModel.UseOfHammer.Value > 0)
                            _playerPrefModel.UseOfHammer.Value--;
                        break;
                    }
                case 1:
                    {
                        if (_playerPrefModel.UseOfStrawberry.Value > 0)
                            _playerPrefModel.UseOfStrawberry.Value--;
                        break;
                    }
                case 2:
                    {
                        if (_playerPrefModel.UseOfFirecracker.Value > 0)
                            _playerPrefModel.UseOfFirecracker.Value--;
                        break;
                    }
            }
        }
        
    }

}
