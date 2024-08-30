using QFramework;
using UnityEngine;
using ZBase.UnityScreenNavigator.Core.Screens;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System;
using ZBase.UnityScreenNavigator.Core.Modals;
using ZBase.UnityScreenNavigator.Core.Views;

public class MenuScreen : ZBase.UnityScreenNavigator.Core.Screens.Screen, IController
{
    [SerializeField] private Button _playBtn;
    [SerializeField] private Button _settingBtn;
    [SerializeField] private Text _coinText;
    [SerializeField] private Text[] _levelText;
    [SerializeField] private GameObject _prevBubbleDisplay;

    private IPlayerPrefModel _prefModel;

    public override async UniTask Initialize(Memory<object> args)
    {
        _prefModel = this.GetModel<IPlayerPrefModel>();

        _playBtn.onClick.RemoveAllListeners();
        _settingBtn.onClick.RemoveAllListeners();
        _coinText.text = _prefModel.Gold.Value.ToString();
        int startIndex = _prefModel.CurrentLevel.Value - 1;
        if (startIndex == 0) _prevBubbleDisplay.SetActive(false);

        foreach (var text in _levelText)
        {
            text.text = startIndex.ToString();
            startIndex++;
        }

        _settingBtn.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayClickSound("click1");
            SettingUI().Forget();
        });
        _playBtn.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayClickSound("click1");
            Play();
            this.SendCommand<RestartLevelCommand>();
        });
    }

    void Play()
    {
        var pageContainer = string.IsNullOrEmpty(ContainerKey.Screens)
                ? ScreenContainer.Of(transform)
                : ScreenContainer.Find(ContainerKey.Screens);
        pageContainer.Pop(true);
    }

    async UniTaskVoid SettingUI()
    {
        var option = new ViewOptions(ResourceKeys.MenuSettingUIPrefab(), true);
        await ModalContainer.Find(ContainerKey.Modals).PushAsync(option);
    }

    IArchitecture IBelongToArchitecture.GetArchitecture()
    {
        return BlueJellyClone.Interface;
    }
}
