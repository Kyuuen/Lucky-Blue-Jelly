using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using UnityEngine;
using ZBase.UnityScreenNavigator.Core.Modals;
using QFramework;
using UnityEngine.UI;

public class BoosterPopup : ZBase.UnityScreenNavigator.Core.Modals.Modal, IController
{
    [SerializeField] private int _type;
    [SerializeField] private Button _buyButton;
    [SerializeField] private Button _adsButton;
    [SerializeField] private Button _closeButton;

    private IPlayerPrefModel _prefModel;
    void Awake()
    {
        _prefModel = this.GetModel<IPlayerPrefModel>();
        _adsButton.onClick.RemoveAllListeners();
        _buyButton.onClick.RemoveAllListeners();
        _closeButton.onClick.RemoveAllListeners();

        if(_prefModel.Gold.Value < 100) _buyButton.interactable = false;

        _adsButton.onClick.AddListener(() =>
        {
            WatchAds();
        });

        _buyButton.onClick.AddListener(() =>
        {
            Buy();
        });

        _closeButton.onClick.AddListener(() =>
        {
            Close();
        });

    }

    void WatchAds()
    {
        //attach ads event here
        switch (_type)
        {
            case 0:
                {
                    _prefModel.UseOfHammer.Value += 1;
                    break;
                }
            case 1:
                {
                    _prefModel.UseOfStrawberry.Value+=1;
                    break;
                }
            case 2:
                {
                    _prefModel.UseOfFirecracker.Value += 1;
                    break;
                }
        }
        Close();
    }

    void Buy()
    {
        _prefModel.Gold.Value -= 100;
        switch(_type)
        {
            case 0:
                {
                    _prefModel.UseOfHammer.Value++;
                    break;
                }
            case 1:
                {
                    _prefModel.UseOfStrawberry.Value++;
                    break;
                }
            case 2:
                {
                    _prefModel.UseOfFirecracker.Value++;
                    break;
                }
        }
        Close();

    }

    void Close()
    {
        var modalContainer = string.IsNullOrEmpty(ContainerKey.Modals)
                ? ModalContainer.Of(transform)
                : ModalContainer.Find(ContainerKey.Modals);
        modalContainer.Pop(false);
        this.SendCommand(new BoosterInactivateCommand
        {
            _isPopupOn = true
        });
    }

    IArchitecture IBelongToArchitecture.GetArchitecture()
    {
        return BlueJellyClone.Interface;
    }
}
