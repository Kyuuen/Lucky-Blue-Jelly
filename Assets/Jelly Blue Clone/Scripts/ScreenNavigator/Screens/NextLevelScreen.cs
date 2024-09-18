using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZBase.UnityScreenNavigator.Core.Screens;
using QFramework;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System;

public class NextLevelScreen : ZBase.UnityScreenNavigator.Core.Screens.Screen, IController
{
    [SerializeField] private Text _coinText;
    [SerializeField] private Button _nextLevel;
    [SerializeField] private Text[] _levelText;
    [SerializeField] private GameObject _hidenPlate;

    private IPlayerPrefModel _prefModel;
    public override async UniTask Initialize(Memory<object> args)
    {
        _prefModel = this.GetModel<IPlayerPrefModel>();

        _nextLevel.onClick.RemoveAllListeners();
        _nextLevel.enabled = false;
        _coinText.text = _prefModel.Gold.Value.ToString();
        int startIndex = _prefModel.CurrentLevel.Value - 1;
        foreach (var text in _levelText)
        {
            text.text = startIndex.ToString();
            startIndex++;
        }
        if (_prefModel.CurrentLevel.Value == 1) _hidenPlate.SetActive(true);
    }

    void Start()
    {
        _nextLevel.enabled = true;
        _nextLevel.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayClickSound("click1");
            NextLevel();
            
        });

        UniTask.Create(async () =>
        {
            await UniTask.Delay(2700);
            if (_levelText[3] != null) { _levelText[3].color = Color.white; }
        });
    }

    void Pop()
    {
        var pageContainer = string.IsNullOrEmpty(ContainerKey.Screens)
                ? ScreenContainer.Of(transform)
                : ScreenContainer.Find(ContainerKey.Screens);
        pageContainer.Pop(true);
    }

    void NextLevel()
    {
        Pop();
        this.SendCommand<GetNextLevelCommand>();
    }

    IArchitecture IBelongToArchitecture.GetArchitecture()
    {
        return BlueJellyClone.Interface;
    }
}
