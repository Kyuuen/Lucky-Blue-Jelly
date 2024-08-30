using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using ZBase.UnityScreenNavigator.Core.Modals;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using ZBase.UnityScreenNavigator.Core.Views;
using ZBase.UnityScreenNavigator.Core.Screens;

public class SettingUI : ZBase.UnityScreenNavigator.Core.Modals.Modal, IController
{
    [SerializeField] private Button closeBtn;
    [SerializeField] private Button restartBtn;
    [SerializeField] private Button mainMenuBtn;
    [SerializeField] private Button soundBtn;
    [SerializeField] private Button vibrateBtn;

    [SerializeField] Sprite soundIcon1; // Icon ban đầu
    [SerializeField] Sprite soundIcon2; // Icon khi click

    [SerializeField] Image soundImage; // Image component của nút
    private bool soundIsIcon1 = true; // Biến để theo dõi trạng thái

    [SerializeField] Sprite vibrateIcon1; // Icon ban đầu
    [SerializeField] Sprite vibrateIcon2; // Icon khi click

    [SerializeField] Image vibrateImage; // Image component của nút
    private bool vibrateIsIcon1 = true; // Biến để theo dõi trạng thái

    private IPlayerPrefModel _prefModel;

    protected override void Awake()
    {
        _prefModel = this.GetModel<IPlayerPrefModel>();
        closeBtn.onClick.RemoveAllListeners();
        soundBtn.onClick.RemoveAllListeners();
        vibrateBtn.onClick.RemoveAllListeners();

        soundIsIcon1 = _prefModel.SoundAvailable.Value;
        vibrateIsIcon1 = _prefModel.VibrateAvailable.Value;

        ChangeSoundIcon(!soundIsIcon1);
        ChangeVibrateIcon(!vibrateIsIcon1);

        if(restartBtn != null && mainMenuBtn != null)
        {
            restartBtn.onClick.RemoveAllListeners();
            mainMenuBtn.onClick.RemoveAllListeners();
            mainMenuBtn.onClick.AddListener(() =>
            {
                AudioManager.Instance.PlayClickSound("click1");
                MainMenu().Forget();
            });
            restartBtn.onClick.AddListener(() =>
            {
                AudioManager.Instance.PlayClickSound("click1");
                CloseSetting();
                this.SendCommand<RestartLevelCommand>();
            });
        }
        
        closeBtn.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayClickSound("click2");
            CloseSetting();
            this.SendCommand(new GamePauseCommand
            {
                gameIsPaused = false
            });
        });
        soundBtn.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayClickSound("click1");
            AudioManager.Instance.ToggleSound(!soundIsIcon1);
            
            ChangeSoundIcon(soundIsIcon1);
            // Cập nhật trạng thái
            soundIsIcon1 = !soundIsIcon1;
            _prefModel.SoundAvailable.Value = !_prefModel.SoundAvailable.Value;
        }); 

        vibrateBtn.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayClickSound("click1");
            this.SendCommand(new VibrationEnableCommand { vibrationIsEnable = !vibrateIsIcon1 });
            ChangeVibrateIcon(vibrateIsIcon1);
            // Cập nhật trạng thái
            vibrateIsIcon1 = !vibrateIsIcon1;
            _prefModel.VibrateAvailable.Value = !_prefModel.VibrateAvailable.Value;
        });

    }

    void ChangeSoundIcon(bool isIcon1)
    {
        // Đổi icon khi click
        if (isIcon1)
        {
            soundImage.sprite = soundIcon2;
        }
        else
        {
            soundImage.sprite = soundIcon1;
        } 
    }

    void ChangeVibrateIcon(bool isIcon1)
    {
        // Đổi icon khi click
        if (isIcon1)
        {
            vibrateImage.sprite = vibrateIcon2;
        }
        else
        {
            vibrateImage.sprite = vibrateIcon1;
        }
    }

    void CloseSetting() 
    {
        var modalContainer = string.IsNullOrEmpty(ContainerKey.Modals)
                ? ModalContainer.Of(transform)
                : ModalContainer.Find(ContainerKey.Modals);
        modalContainer.Pop(true);
    }

    async UniTaskVoid MainMenu()
    {
        var option = new ViewOptions(ResourceKeys.MenuScenePrefab(), true);
        await ScreenContainer.Find(ContainerKey.Screens).PushAsync(option);
        this.SendCommand<ToMenuCommand>();
        CloseSetting();
    }

    IArchitecture IBelongToArchitecture.GetArchitecture()
    {
        return BlueJellyClone.Interface;
    }
}
