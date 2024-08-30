using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZBase.UnityScreenNavigator.Core.Modals;
using QFramework;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using ZBase.UnityScreenNavigator.Core.Screens;
using ZBase.UnityScreenNavigator.Core.Views;
using TMPro;

public class LevelCompletedUI : ZBase.UnityScreenNavigator.Core.Modals.Modal, IController
{
    [SerializeField] private Button closeBtn;
    [SerializeField] private Button claimBtn;
    [SerializeField] private Button doubleBtn;
    [SerializeField] private TMP_Text levelText;

    private IGoldSystem goldSystem;
    private IPlayerPrefModel playerPrefModel;

    protected override void Awake()
    {
        goldSystem = this.GetSystem<IGoldSystem>();
        playerPrefModel = this.GetModel<IPlayerPrefModel>();

        closeBtn.onClick.RemoveAllListeners();
        claimBtn.onClick.RemoveAllListeners();
        doubleBtn.onClick.RemoveAllListeners();
        doubleBtn.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayClickSound("click1");
            PentaCoinAndTranslate();
        });
        claimBtn.onClick.AddListener(() => 
        {
            AudioManager.Instance.PlayClickSound("click1");
            ClaimAndTranslate().Forget();
        });
        closeBtn.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayClickSound("click2");
            CloseUI();
            this.SendCommand<RestartLevelCommand>();
        });
        levelText.text = "Level " + playerPrefModel.CurrentLevel.Value.ToString();
    }

    void CloseUI()
    {
        var modalContainer = string.IsNullOrEmpty(ContainerKey.Modals)
                ? ModalContainer.Of(transform)
                : ModalContainer.Find(ContainerKey.Modals);
        modalContainer.Pop(true);
    }

    async UniTaskVoid ClaimAndTranslate()
    {
        goldSystem.GetExtraGold(1);
        CloseUI();
        var option = new ViewOptions(ResourceKeys.NextLevelUIPrefab(), true);
        await ScreenContainer.Find(ContainerKey.Screens).PushAsync(option);
        this.SendCommand<ToMenuCommand>();
    }

    async void PentaCoinAndTranslate()
    {
        goldSystem.GetExtraGold(5);
        CloseUI();
        //attach ads here
        var option = new ViewOptions(ResourceKeys.NextLevelUIPrefab(), true);
        await ScreenContainer.Find(ContainerKey.Screens).PushAsync(option);
        this.SendCommand<ToMenuCommand>();
    }

    IArchitecture IBelongToArchitecture.GetArchitecture()
    {
        return BlueJellyClone.Interface;
    }
}
