using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using Unity.VisualScripting;
using System.Runtime.CompilerServices;
using NaughtyAttributes;
using TMPro;

public class IceController : MonoBehaviour, IController
{
    [HideInInspector] public int id;

    [SerializeField] private bool _isLocked;
    [ShowIf("_isLocked")]
    [SerializeField] private int _requestPoint;
    [ShowIf("_isLocked")]
    [SerializeField] private TMP_Text _lockText;
    [ShowIf("_isLocked")]
    [SerializeField] private GameObject _lock;

    [SerializeField] private GameObject _graphic;

    int currentState = 0;
    SpriteRenderer spriteRenderer;

    IGameSceneModel _gameSceneModel;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();


        if (_isLocked)
        {
            id = 10000;
            _lockText.text = _requestPoint.ToString();
            _gameSceneModel = this.GetModel<IGameSceneModel>();
            _gameSceneModel.Score.Register(OnChangedValue).UnRegisterWhenGameObjectDestroyed(gameObject);
        }
        else
        {
            this.RegisterEvent<BreakObstaclesEvent>(e =>
            {
                if (e.iceIds != null)
                {
                    foreach (var id in e.iceIds)
                    {
                        if (this.id == id)
                        {
                            ChangeState();
                            break;
                        }
                    }
                }

            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<FirecrackerDestroyEvent>(e =>
            {
                if (e._iceIds != null && !_isLocked)
                {
                    foreach (var id in e._iceIds)
                    {
                        if (this.id == id)
                        {
                            PlayAnimation();
                            break;
                        }
                    }
                }
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

        }
    }

    void OnChangedValue(int score)
    {
        if (score >= _requestPoint)
        {
            _lock.SetActive(false);
            PlayAnimation();
        }
    }

    public void ChangeState()
    {
        if (currentState == 0)
        {
            currentState = 1;
            spriteRenderer.sprite = Resources.Load<Sprite>(ResourceKeys.IceSpriteSource(currentState));
        }
        else if (currentState == 1)
        {
            currentState++;
            PlayAnimation();
        }
    }

    void PlayAnimation()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        //Debug.Log($"{GetComponent<SpriteRenderer>().enabled}");
        _graphic.SetActive(true);
        _graphic.GetComponent<Animator>().SetBool("isBreak", true);
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
        Destroy(gameObject, 2.5f);
    }

    IArchitecture IBelongToArchitecture.GetArchitecture()
    {
        return BlueJellyClone.Interface;
    }
}
