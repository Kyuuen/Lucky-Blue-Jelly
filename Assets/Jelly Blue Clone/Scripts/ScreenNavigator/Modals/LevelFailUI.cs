using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZBase.UnityScreenNavigator.Core.Modals;
using QFramework;
using UnityEngine.UI;
using TMPro;

public class LevelFailUI : ZBase.UnityScreenNavigator.Core.Modals.Modal, IController
{
    [SerializeField] private Button closeBtn;
    [SerializeField] private Button restartBtn;
    [SerializeField] private Button playOnBtn;
    [SerializeField] private TMP_Text levelText;

    private IPlayerPrefModel playerPrefModel;
    protected override void Awake()
    {
        playerPrefModel = this.GetModel<IPlayerPrefModel>();
        closeBtn.onClick.RemoveAllListeners();
        playOnBtn.onClick.RemoveAllListeners();
        restartBtn.onClick.RemoveAllListeners();
        restartBtn.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayClickSound("click1");
            CloseUI();
            this.SendCommand<RestartLevelCommand>();
        });
        playOnBtn.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayClickSound("click1");
            PLayOn();
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

    void PLayOn()
    {
        CloseUI();
        this.SendCommand<PlayOnCommand>();
    }
    

    IArchitecture IBelongToArchitecture.GetArchitecture()
    {
        return BlueJellyClone.Interface;
    }
}
