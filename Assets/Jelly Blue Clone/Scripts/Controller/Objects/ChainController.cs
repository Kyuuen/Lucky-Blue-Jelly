using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using TMPro;
using Cysharp.Threading.Tasks;

public class ChainSimulateController : MonoBehaviour, IController
{
    [SerializeField] private int _scoreRequire;
    [SerializeField] private List<GameObject> _chainList;
    [SerializeField] private GameObject _lock;
    [SerializeField] private TMP_Text _textMesh;

    private IGameSceneModel _gameSceneModel;
    void Awake()
    {
        _gameSceneModel = this.GetModel<IGameSceneModel>();

        foreach (Transform child in transform)
        {
            _chainList.Add(child.gameObject);
        }
        _textMesh.text = _scoreRequire.ToString();

        _gameSceneModel.Score.Register(OnScoreReach).UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    void OnScoreReach(int score)
    {
        if (_scoreRequire <= score) 
        {
            UniTask.Create(async () =>
            {
                await UniTask.Delay(2000);
                _lock.GetComponent<Animator>().SetBool("isUnlock", true);
                Destroy(_lock, 1.5f);
                BreakChain();
            });
        }
    }

    async void BreakChain()
    {
        await UniTask.Delay(1000);
        int breakPoint = _chainList.Count / 2;
        _chainList[breakPoint].GetComponent<HingeJoint2D>().enabled = false;
        Destroy(gameObject, 1f);
    }

    IArchitecture IBelongToArchitecture.GetArchitecture()
    {
        return BlueJellyClone.Interface;
    }
}
