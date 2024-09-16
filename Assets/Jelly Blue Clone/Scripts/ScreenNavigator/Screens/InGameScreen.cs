using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZBase.UnityScreenNavigator.Core.Screens;
using ZBase.UnityScreenNavigator.Core;
using QFramework;
using ZBase.UnityScreenNavigator.Core.Views;
using Cysharp.Threading.Tasks;
using ZBase.UnityScreenNavigator.Core.Modals;
using System;
using UnityEngine.UI;
using System.Resources;
using ZBase.UnityScreenNavigator.Core.Activities;
using Cysharp.Threading.Tasks.CompilerServices;

public class InGameScreen : ZBase.UnityScreenNavigator.Core.Screens.Screen, IController
{
    [SerializeField] private Button settingBtn;

    public override async UniTask Initialize(Memory<object> args)
    {
        this.RegisterEvent<PostLevelWinEvent>(PostLevelCompleted).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<GameOverEvent>(GameOver).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<BoosterActivateEvent>(BoosterActivate).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<BoosterPopupActivateEvent>(BoosterPopupActivate).UnRegisterWhenGameObjectDestroyed(gameObject);

        settingBtn.onClick.RemoveAllListeners();
        settingBtn.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayClickSound("click1");
            SettingUI().Forget();
            this.SendCommand(new GamePauseCommand
            {
                gameIsPaused = true
            });
        });
    }

    void PostLevelCompleted(PostLevelWinEvent e)
    {
        PostLevelCompleted().Forget();
    }

    void GameOver(GameOverEvent e)
    {
        GameOver().Forget();
    }

    void BoosterActivate(BoosterActivateEvent e)
    {
        BoosterUI(e._boosterStatus).Forget();
    }

    async void BoosterPopupActivate(BoosterPopupActivateEvent e)
    {
        switch (e._status) 
        {
            case 0:
                {
                    var option = new ViewOptions(ResourceKeys.HammerBoosterPopupPrefab(), true);
                    await ModalContainer.Find(ContainerKey.Modals).PushAsync(option);
                    break;
                }
            case 1:
                {
                    var option = new ViewOptions(ResourceKeys.StrawberryBoosterPopupPrefab(), true);
                    await ModalContainer.Find(ContainerKey.Modals).PushAsync(option);
                    break;
                }
            case 2:
                {
                    var option = new ViewOptions(ResourceKeys.FirecrackerBoosterPopupPrefab(), true);
                    await ModalContainer.Find(ContainerKey.Modals).PushAsync(option);
                    break;
                }
        }
    }

    async UniTaskVoid BoosterUI(int status)
    {
        var option = new ViewOptions(ResourceKeys.BoosterUIPrefab(status), false);
        await ModalContainer.Find(ContainerKey.Modals).PushAsync(option);
    }

    async UniTaskVoid SettingUI()
    {
        var option = new ViewOptions(ResourceKeys.SettingUIPrefab(), true);
        await ModalContainer.Find(ContainerKey.Modals).PushAsync(option);
    }

    async UniTaskVoid PostLevelCompleted()
    {
        var option = new ViewOptions(ResourceKeys.LevelCompletedPrefab(), true);
        await UniTask.Delay(2000);
        await ModalContainer.Find(ContainerKey.Modals).PushAsync(option);
    }

    async UniTaskVoid GameOver()
    {
        var modalContainer = string.IsNullOrEmpty(ContainerKey.Modals)
                ? ModalContainer.Of(transform)
                : ModalContainer.Find(ContainerKey.Modals);
        modalContainer.Pop(false);
        this.SendCommand(new BoosterInactivateCommand
        {
            _isPopupOn = true
        });
        var option = new ViewOptions(ResourceKeys.LevelFailPrefab(), true);
        await ModalContainer.Find(ContainerKey.Modals).PushAsync(option);
    }

    IArchitecture IBelongToArchitecture.GetArchitecture()
    {
        return BlueJellyClone.Interface;
    }
}
