
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using UnityEngine.UI;
using TMPro;
using System.Xml.Linq;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using System.Collections;

public class GamePanel : MonoBehaviour, IController
{
    [SerializeField] private Text _move;
    [SerializeField] private Text _scores;
    [SerializeField] private Text _gold;
    [SerializeField] private RectTransform _scoreImageTrans;
    [SerializeField] private RectTransform _goldImageTrans;
    [SerializeField] private Button[] _boosterBtns;
    [SerializeField] private Button _switchBtns;

    private LevelObjectSpawner _thisLevel;
    private int _currentLevelIndex = 1;
    private int _oldGoldvValue;

    [SerializeField] private GameObject _jellyUI;

    private IScoreSystem _scoreSystem;
    private IUISystem _uiSystem;

    private IGameSceneModel _gameSceneModel;

    private IPlayerPrefModel _prefModel;

    async void Awake()
    {
        _gameSceneModel = this.GetModel<IGameSceneModel>();
        _prefModel = this.GetModel<IPlayerPrefModel>();

        _scoreSystem = this.GetSystem<IScoreSystem>();
        _uiSystem = this.GetSystem<IUISystem>();

        _oldGoldvValue = _prefModel.Gold.Value;

        if (_prefModel.CurrentLevel.Value <= 20)
        {
            _prefModel.CurrentLevelToLoad.Value = _prefModel.CurrentLevel.Value;
            _currentLevelIndex = _prefModel.CurrentLevel.Value;
            this.SendCommand(new RandomLevelCommand
            {
                _levelIndex = _currentLevelIndex,
            });
        }
        else
        {
            _currentLevelIndex = Random.Range(5, 20);
            _prefModel.CurrentLevelToLoad.Value = _currentLevelIndex;
            this.SendCommand(new RandomLevelCommand
            {
                _levelIndex = _currentLevelIndex,
            });
        }
        _thisLevel = await GetCurrentLevelConfig();

        this.SendCommand<PreGameStartCommand>();

        _uiSystem.MainCanvas = gameObject.GetComponent<RectTransform>();
        _uiSystem.GoldUIPos = _goldImageTrans;
        //_uiSystem.CameraResizer(Camera.main);

        this.RegisterEvent<OnJellyCollectEvent>(OnJellyCollect).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<PostLevelWinEvent>(PostLevelWin).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<OnGameStartEvent>(OnGameStart).UnRegisterWhenGameObjectDestroyed(gameObject);

        _thisLevel = await GetCurrentLevelConfig();

        _gameSceneModel.Score.Register(OnScoreChanged);
        _gameSceneModel.Move.RegisterWithInitValue(OnMoveChanged);
        _prefModel.Gold.RegisterWithInitValue(OnGoldChanged);
        _prefModel.CurrentLevel.RegisterWithInitValue(GetNextLevel);

        AudioManager.Instance.ToggleSound(_prefModel.SoundAvailable.Value);

        OnScoreChanged(_gameSceneModel.Score.Value);
        OnMoveChanged(_gameSceneModel.Score.Value);
        OnGoldChanged(_prefModel.Gold.Value);

        foreach (var button in _boosterBtns)
        {
            button.onClick.RemoveAllListeners(); ;
        }
        _switchBtns.onClick.RemoveAllListeners();


        _boosterBtns[0].onClick.AddListener(() =>
        {
            DisableAllButtons(); // Vô hiệu hóa các nút khi nhấn
            AudioManager.Instance.PlayClickSound("click1");
            ActivateBooster(0, _prefModel.UseOfHammer.Value);
            EnableAllButtons(); // Kích hoạt lại sau khi thực hiện xong
        });
        _boosterBtns[1].onClick.AddListener(() =>
        {
            DisableAllButtons();
            AudioManager.Instance.PlayClickSound("click1");
            ActivateBooster(1, _prefModel.UseOfStrawberry.Value);
            EnableAllButtons();
        });
        _boosterBtns[2].onClick.AddListener(() =>
        {
            DisableAllButtons();
            AudioManager.Instance.PlayClickSound("click1");
            ActivateBooster(2, _prefModel.UseOfFirecracker.Value);
            EnableAllButtons();
        });
        _switchBtns.onClick.AddListener(() =>
        {
            DisableAllButtons();
            AudioManager.Instance.PlayClickSound("click1");
            this.SendCommand<SwitchBubbleCommand>();
            EnableAllButtons();
        });
    }

    // Hàm để vô hiệu hóa tất cả các nút
    void DisableAllButtons()
    {
        foreach (var button in _boosterBtns)
        {
            button.interactable = false;
        }
        _switchBtns.interactable = false;
    }

    // Hàm để kích hoạt lại các nút
    void EnableAllButtons()
    {
        foreach (var button in _boosterBtns)
        {
            button.interactable = true;
        }
        _switchBtns.interactable = true;
    }

    void PostLevelWin(PostLevelWinEvent e)
    {

    }

    void OnGameStart(OnGameStartEvent e)
    {
        OnGoldChanged(_prefModel.Gold.Value);
    }

    void ActivateBooster(int status, int use)
    {
        if (use <= 0)
        {
            this.SendCommand(new BoosterPopupActivateCommand
            {
                status = status
            });
        }
        else
        {
            this.SendCommand(new BoosterActivateCommand
            {
                status = status
            });
        }

    }

    void OnJellyCollect(OnJellyCollectEvent e)
    {
        Vector3 offset = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
        GameObject jellyUI = Instantiate(_jellyUI, e.jelliesScreenPos, Quaternion.identity);
        jellyUI.transform.SetParent(transform);

        Image img = jellyUI.GetComponent<Image>();
        img.sprite = Resources.Load<Sprite>(ResourceKeys.JellySpriteSource(e.type));

        Rigidbody2D rb = jellyUI.AddComponent<Rigidbody2D>();
        rb.DOMove(jellyUI.transform.position + offset, 0.1f).OnComplete(() =>
        {
            rb.DOMove(_scoreImageTrans.position, 0.8f).OnComplete(() =>
            {
                Destroy(jellyUI);
            });
        });
    }

    void OnGoldChanged(int gold)
    {
        //if (!beingInit) await UniTask.Delay(2000);
        StartCoroutine(IncrementToTarget(_oldGoldvValue, gold, 2f));
    }

    IEnumerator IncrementToTarget(int startValue, int endValue, float time)
    {
        float elapsed = 0f;
        int valueDifference = endValue - startValue;

        while (elapsed < time)
        {
            elapsed += Time.deltaTime;
            int newValue = Mathf.RoundToInt(Mathf.Lerp(startValue, endValue, elapsed / time));
            _gold.text = newValue.ToString();
            yield return null;  // Chờ tới frame tiếp theo
        }

        // Đảm bảo giá trị cuối cùng được cập nhật chính xác
        _gold.text = endValue.ToString();
        _oldGoldvValue = endValue;
    }

    void OnScoreChanged(int score)
    {
        //if (!beingInit) await UniTask.Delay(2000);
        _scores.text = score.ToString() + "/" + _thisLevel.ScoreRequest;
        _scoreSystem.CheckScore(_thisLevel.ScoreRequest);
    }

    void OnMoveChanged(int move)
    {
        _move.text = move.ToString();
    }

    async void GetNextLevel(int levelIndex)
    {
        GetLevel(levelIndex);
        _thisLevel = await GetCurrentLevelConfig();
        OnScoreChanged(_gameSceneModel.Score.Value);
        OnMoveChanged(_gameSceneModel.Move.Value);
    }

    void GetLevel(int levelIndex)
    {
        if (levelIndex <= 20)
        {
            _prefModel.CurrentLevelToLoad.Value = levelIndex;
            _currentLevelIndex = levelIndex;
        }
        else
        {
            _currentLevelIndex = Random.Range(5, 20);
            _prefModel.CurrentLevelToLoad.Value = _currentLevelIndex;
            this.SendCommand(new RandomLevelCommand
            {
                _levelIndex = _currentLevelIndex,
            });
        }
    }

    async UniTask<LevelObjectSpawner> GetCurrentLevelConfig()
    {
        UniTask<LevelObjectSpawner> asyncOperationHandle = Addressables.LoadAssetAsync<LevelObjectSpawner>(ResourceKeys.CurrentLevelPrefab(_currentLevelIndex)).Task.AsUniTask();
        LevelObjectSpawner result = await asyncOperationHandle;
        return result;
    }

    IArchitecture IBelongToArchitecture.GetArchitecture()
    {
        return BlueJellyClone.Interface;
    }
}
