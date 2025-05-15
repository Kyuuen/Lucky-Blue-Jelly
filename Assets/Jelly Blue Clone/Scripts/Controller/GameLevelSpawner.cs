using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;

public class GameLevelSpawner : MonoBehaviour, IController
{
    [SerializeField] private int testLevel;
    private GameObject _currentLevel;
    private int _currentLevelIndex;

    private LevelObjectSpawner _thisLevel;
    private GameObject _currentInstanceLevel;

    private IPlayerPrefModel _prefModel;
    private IGameSceneModel _gameSceneModel;

    void Awake()
    {
        _prefModel = this.GetModel<IPlayerPrefModel>();
        _gameSceneModel = this.GetModel<IGameSceneModel>();

        _prefModel.SetToTest(testLevel);

        this.RegisterEvent<PreGameStartEvent>(async e =>
        {
            _currentLevel = await GetCurrentLevel();
            _thisLevel = await GetCurrentLevelConfig();
            _currentInstanceLevel = Instantiate(_currentLevel);
            this.SendCommand(new OnGameStartCommand
            {
                _move = _thisLevel.Move
            });
        });

        this.RegisterEvent<RandomLevelEvent>(RandomLevel).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<RestartLevelEvent>(async e =>
        {
            Destroy(_currentInstanceLevel);
            _gameSceneModel.ResetModel();
            await UniTask.NextFrame();
            _currentInstanceLevel = Instantiate(_currentLevel);
            this.SendCommand(new OnGameStartCommand
            {
                _move = _thisLevel.Move
            });
        });
        _prefModel.CurrentLevel.Register(LoadNewLevel);
        this.RegisterEvent<ToMenuEvent>(e =>
        {
            Destroy(_currentInstanceLevel);
            _gameSceneModel.ResetModel();
        });

    }

    void LoadNewLevel(int currentLevel) //need event preference
    {
        if (currentLevel <= 20)
        {
            _currentLevelIndex = currentLevel;
        }
        else
        {
            _currentLevelIndex = Random.Range(5, 20);
        }
        SpawnNewLevel();
    }

    public void RandomLevel(RandomLevelEvent e)
    {
        _currentLevelIndex = e._levelIndex;
    }

    async void SpawnNewLevel()
    {
        Destroy(_currentInstanceLevel);
        _gameSceneModel.ResetModel();
        await UniTask.NextFrame();
        _currentLevel = await GetCurrentLevel();
        _thisLevel = await GetCurrentLevelConfig();
        _currentInstanceLevel = Instantiate(_currentLevel);
        this.SendCommand(new OnGameStartCommand
        {
            _move = _thisLevel.Move
        });
    }

    async UniTask<LevelObjectSpawner> GetCurrentLevelConfig()
    {
        UniTask<LevelObjectSpawner> asyncOperationHandle = Addressables.LoadAssetAsync<LevelObjectSpawner>(ResourceKeys.CurrentLevelPrefab(_currentLevelIndex)).Task.AsUniTask();
        LevelObjectSpawner result = await asyncOperationHandle;
        return result;
    }

    async UniTask<GameObject> GetCurrentLevel()
    {
        UniTask<GameObject> asyncOperationHandle = Addressables.LoadAssetAsync<GameObject>(ResourceKeys.CurrentLevelPrefab(_currentLevelIndex)).Task.AsUniTask();
        GameObject result = await asyncOperationHandle;
        return result;
    }

    IArchitecture IBelongToArchitecture.GetArchitecture()
    {
        return BlueJellyClone.Interface;
    }
}
