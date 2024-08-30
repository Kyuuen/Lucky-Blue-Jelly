using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZBase.UnityScreenNavigator.Core.Modals;
using QFramework;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System;

public class HammerBoosterUI : ZBase.UnityScreenNavigator.Core.Modals.Modal, IController
{
    [SerializeField] private Button _cancelBtn;

    protected override void Awake()
    {
        _cancelBtn.onClick.RemoveAllListeners();
        _cancelBtn.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayClickSound("click2");
            this.SendCommand(new BoosterInactivateCommand { _isPopupOn = true });
            Cancel();
        });
    }

    void Cancel()
    {
        var modalContainer = string.IsNullOrEmpty(ContainerKey.Modals)
                ? ModalContainer.Of(transform)
                : ModalContainer.Find(ContainerKey.Modals);
        modalContainer.Pop(false);
        
    }

    IArchitecture IBelongToArchitecture.GetArchitecture()
    {
        return BlueJellyClone.Interface;
    }
}
