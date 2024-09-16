using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using UnityEngine.UI;
using TMPro;

public class BoosterButtons : MonoBehaviour, IController
{
    [SerializeField] private int _boosterType;
    [SerializeField] private GameObject _lockImg;
    [SerializeField] private GameObject _unlockImg;
    [SerializeField] private int _unlockLevel;
    [SerializeField] private TMP_Text _useText;

    private IPlayerPrefModel _prefModel;
    void Awake()
    {
        _lockImg.SetActive(true);
        _unlockImg.SetActive(false);
        _useText.text = string.Empty;
        GetComponent<Button>().interactable = false;

        _prefModel = this.GetModel<IPlayerPrefModel>();

        _prefModel.CurrentLevel.Register(OnLevelChange).UnRegisterWhenGameObjectDestroyed(gameObject);
        OnLevelChange(_prefModel.CurrentLevel.Value);

        switch (_boosterType)
        {
            case 0:
                {
                    _prefModel.UseOfHammer.Register(OnUseChange).UnRegisterWhenGameObjectDestroyed(gameObject);
                    OnUseChange(_prefModel.UseOfHammer.Value);
                    break;
                }
            case 1:
                {
                    _prefModel.UseOfStrawberry.Register(OnUseChange).UnRegisterWhenGameObjectDestroyed(gameObject);
                    OnUseChange(_prefModel.UseOfStrawberry.Value);
                    break;
                }
            case 2:
                {
                    _prefModel.UseOfFirecracker.Register(OnUseChange).UnRegisterWhenGameObjectDestroyed(gameObject);
                    OnUseChange(_prefModel.UseOfFirecracker.Value);
                    break;
                }
        }
    }

    void OnUseChange(int use)
    {
        if (use == 0 && _prefModel.CurrentLevel.Value < _unlockLevel)
        {
            _useText.text = string.Empty;
            return;
        }
        _useText.text = use.ToString();
    }

    void OnLevelChange(int level)
    {
        if (_prefModel.CurrentLevel.Value >= _unlockLevel)
        {
            _lockImg.SetActive(false);
            _unlockImg.SetActive(true);
            GetComponent<Button>().interactable = true;
        }
        if (_prefModel.CurrentLevel.Value == _unlockLevel)
        {
            switch (_boosterType)
            {
                case 0:
                    {
                        _prefModel.UseOfHammer.Value = 1;
                        break;
                    }
                case 1:
                    {
                        _prefModel.UseOfStrawberry.Value = 1;
                        break;
                    }
                case 2:
                    {
                        _prefModel.UseOfFirecracker.Value = 1;
                        break;
                    }
            }
        }
    }

    IArchitecture IBelongToArchitecture.GetArchitecture()
    {
        return BlueJellyClone.Interface;
    }
}
