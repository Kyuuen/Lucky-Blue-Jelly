 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;

public class GameLevelSpawner : MonoBehaviour, IController
{
    //[SerializeField] private int testLevel; 
    private GameObject currentLevel;
    int currentLevelIndex;

    private LevelObjectSpawner _thisLevel;
    private GameObject _currentInstanceLevel;

    private IPlayerPrefModel _prefModel;
    private IGameSceneModel _gameSceneModel;

    async void Awake()
    {
        _prefModel = this.GetModel<IPlayerPrefModel>();
        _gameSceneModel = this.GetModel<IGameSceneModel>();

        //_prefModel.Reset();
        //_prefModel.SetToTest(testLevel);
        //_prefModel.ResetBooster();
        currentLevelIndex = _prefModel.CurrentLevel.Value;
        currentLevel = await GetCurrentLevel();
        _thisLevel = await GetCurrentLevelConfig();
        this.RegisterEvent<PreGameStartEvent>(e =>
        {
            _currentInstanceLevel = Instantiate(currentLevel);
            this.SendCommand(new OnGameStartCommand
            {
                move = _thisLevel.Move
            });
        });
        this.RegisterEvent<RestartLevelEvent>(e =>
        { 
            SpawnNewLevel();
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
        if(currentLevel <= 20)
        {
            currentLevelIndex = currentLevel;
        }
        else
        {
            currentLevelIndex = Random.Range(2, 20);
        }
        SpawnNewLevel();
    }

    async void SpawnNewLevel()
    {
        Destroy(_currentInstanceLevel);
        _gameSceneModel.ResetModel();
        await UniTask.NextFrame();
        currentLevel = await GetCurrentLevel();
        _thisLevel = await GetCurrentLevelConfig();
        _currentInstanceLevel = Instantiate(currentLevel);
        this.SendCommand(new OnGameStartCommand
        {
            move = _thisLevel.Move
        });
    }

    async UniTask<LevelObjectSpawner> GetCurrentLevelConfig()
    {
        UniTask<LevelObjectSpawner> asyncOperationHandle = Addressables.LoadAssetAsync<LevelObjectSpawner>(ResourceKeys.CurrentLevelPrefab(currentLevelIndex)).Task.AsUniTask();
        LevelObjectSpawner result = await asyncOperationHandle;
        return result;
    }

    async UniTask<GameObject> GetCurrentLevel()
    {
        UniTask<GameObject> asyncOperationHandle = Addressables.LoadAssetAsync<GameObject>(ResourceKeys.CurrentLevelPrefab(currentLevelIndex)).Task.AsUniTask();
        GameObject result = await asyncOperationHandle;
        return result;
    }

    IArchitecture IBelongToArchitecture.GetArchitecture()
    {
        return BlueJellyClone.Interface;
    }
}
