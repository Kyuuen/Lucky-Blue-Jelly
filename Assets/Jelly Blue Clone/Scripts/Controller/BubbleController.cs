using Cysharp.Threading.Tasks;
using DG.Tweening;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;
using NaughtyAttributes;
using TMPro;
using ZBase.UnityScreenNavigator.Core.Modals;

public interface IBubbleController : IController
{

}
public class BubbleController : MonoBehaviour, IBubbleController
{
    public int iD;
    public List<GameObject> jellies;
    [HideInInspector] public int maxNumb;

    [SerializeField] private bool _isLocked;
    [ShowIf("_isLocked")]
    [SerializeField] private int _requestPoint;
    [ShowIf("_isLocked")]
    [SerializeField] private TMP_Text _lockText;

    //[SerializeField] private LayerMask _cloneMask;
    //[SerializeField] private LayerMask _ignoreMask;

    [HideInInspector] public float distance;
    [HideInInspector] public Vector3 extents;
    [HideInInspector] public bool isDone;

    [HideInInspector] public bool _firstHit = false;
    [HideInInspector] public bool _spawnEarly;
    private bool _gameIsEnd;
    [HideInInspector] public Rigidbody2D _rb;

    [SerializeField] private Animator _animator;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    [SerializeField] private LayerMask _iceMask;
    [SerializeField] private LayerMask _wireMask;

    private bool _boosterIsOn;
    private int _boosterStatus;
    private bool _clickable;

    private IMergeBubbleSystem _mergeBubbleSystem;

    private IGameSceneModel _gameSceneModel;

    private void Awake()
    {
        Vibration.Init();
        _gameSceneModel = this.GetModel<IGameSceneModel>();
        _mergeBubbleSystem = this.GetSystem<IMergeBubbleSystem>();
        _gameIsEnd = false;

        _spawnEarly = false;
        _rb = GetComponent<Rigidbody2D>();
        _animator.SetBool("isCracked", false);

        isDone = false;

        extents = _spriteRenderer.bounds.extents;
        distance = extents.magnitude - 1.2f;
        _boosterIsOn = false;
        _clickable = true;

        if (_isLocked)
        {
            iD = 100000;
            _lockText.text = _requestPoint.ToString();
            _gameSceneModel.Score.Register(OnScoreChanged).UnRegisterWhenGameObjectDestroyed(gameObject);
        }
        else
        {
            this.RegisterEvent<FirecrackerDestroyEvent>(e =>
            {
                if (e._bubbleIds != null && !_isLocked)
                {
                    foreach (var id in e._bubbleIds)
                    {
                        if (iD == id)
                        {
                            IsDone();
                            break;
                        }
                    }
                }
            });
        }
        this.RegisterEvent<BoosterInactivateEvent>(e =>
        {
            _boosterIsOn = false;
            BoosterDeactivate();
        }).UnRegisterWhenGameObjectDestroyed(gameObject);

        this.RegisterEvent<OnLevelWinEvent>(e =>
        {
            _gameIsEnd = true;
        });
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_gameIsEnd || isDone) return;
        if (collision.gameObject.layer == 3 && !_isLocked && !collision.gameObject.GetComponent<BubbleController>()._isLocked)
        {
            _mergeBubbleSystem.GetBubbleContact(iD, collision.gameObject.GetComponent<BubbleController>().iD);
        }
        if (!_firstHit)
        {
            if (!collision.gameObject.CompareTag("Bubble") && !collision.gameObject.CompareTag("Map") && !collision.gameObject.CompareTag("LevelObstacle"))
            {
                return;
            }
        }

    }

    public void IsSpawnEarly()
    {
        _spawnEarly = !_spawnEarly;
    }

    public void ConnectToBubble()
    {
        foreach (var jelly in jellies)
        {
            jelly.GetComponent<JellyController>().ConnectToBubble(_rb, distance);
            jelly.GetComponent<JellyController>().ReduceColliderRadius(0.01f * jellies.Count);
        }
    }

    public void DisConnectToBubble()
    {
        foreach (var jelly in jellies)
        {
            if (jelly != null)
            {
                jelly.GetComponent<JellyController>().DisConnectToBubble();
                jelly.GetComponent<Rigidbody2D>().drag = 100f;
            }

        }
    }

    public void DisConnectToType(int type, bool removeThatType)
    {
        foreach (var jelly in jellies)
        {
            if (removeThatType)
            {
                if (jelly.GetComponent<JellyController>().Type == type)
                    jelly.GetComponent<JellyController>().DisConnectToBubble();
            }
            else
            {
                if (jelly.GetComponent<JellyController>().Type != type)
                    jelly.GetComponent<JellyController>().DisConnectToBubble();
            }
        }
    }

    void OnScoreChanged(int score)
    {
        if (score >= _requestPoint)
        {
            _animator.SetBool("isCracked", true);
            Destroy(gameObject, 1.8f);
        }
    }

    public void BoosterActivate(int status)
    {
        if (_rb.bodyType == RigidbodyType2D.Static) return;
        if (status == 0 || status == 1)
        {
            _boosterIsOn = true;
            _spriteRenderer.sortingLayerName = "UI";
            _spriteRenderer.sortingOrder = 2001;
            _boosterStatus = status;
            foreach (var jelly in jellies)
            {
                jelly.GetComponent<JellyController>().ActivateBooster();
            }
        }
    }

    public void ChangeMask(bool isNotDropped)
    {
        if (isNotDropped)
        {
            gameObject.layer = 10;
            foreach (GameObject jelly in jellies)
            {
                jelly.layer = 11;
            }
        }
        else
        {
            gameObject.layer = 3;
            foreach (GameObject jelly in jellies)
            {
                jelly.layer = 0;
            }
        }
    }

    public void BoosterDeactivate()
    {
        _boosterIsOn = false;
        _spriteRenderer.sortingLayerName = "Default";
        _spriteRenderer.sortingOrder = 5;
        _boosterStatus = -1;
        foreach (var jelly in jellies)
        {
            jelly.GetComponent<JellyController>().DeactivateBooster();
        }
    }

    public void BoosterClickOn()
    {
        if (!_boosterIsOn || !_clickable) return;
        else
        {
            if (_boosterStatus == 0)
            {
                var modalContainer = string.IsNullOrEmpty(ContainerKey.Modals)
                    ? ModalContainer.Of(transform)
                    : ModalContainer.Find(ContainerKey.Modals);
                modalContainer.Pop(false);
                this.SendCommand(new PlayHammerAniCommand
                {
                    iD = iD,
                    bubble = transform
                });
                _clickable = false;
            }
            if (_boosterStatus == 1)
            {
                var modalContainer = string.IsNullOrEmpty(ContainerKey.Modals)
                    ? ModalContainer.Of(transform)
                    : ModalContainer.Find(ContainerKey.Modals);
                modalContainer.Pop(false);
                this.SendCommand(new PlayStrawberryAniCommand
                {
                    iD = iD
                });
                _clickable = false;
            }
        }
    }

    public void PrevDone(int maxNumb)
    {
        if (maxNumb < jellies.Count)
        {
            for (int i = maxNumb; i < jellies.Count; i++)
            {
                Destroy(jellies[i]);
            }
        }
    }

    public async void IsDone()
    {
        isDone = true;
        if (_animator != null) _animator.SetBool("isCracked", true);
        if (_rb != null)
        {
            _rb.gravityScale = 0;
            _rb.drag = 1000;
        }
        foreach (var jelly in jellies)
        {
            if (jelly != null)
                jelly.GetComponent<JellyController>().GetBubbleTransform(transform);
        }
        await UniTask.Delay(500);
        AudioManager.Instance.PlayBubbleSound(3);


        DisConnectToBubble();
        //GetComponent<SpriteRenderer>().enabled = false;

        foreach (var jelly in jellies)
        {
            if (jelly != null)
                jelly.GetComponent<JellyController>().OnBeingCollected();
        }

        jellies.Clear();
        this.SendCommand(new BreakObstaclesCommand
        {
            iceIds = FindObjectsInRadius(_iceMask),
            wireIds = FindObjectsInRadius(_wireMask)
        });
        Destroy(gameObject, 0.8f);
    }

    public void IsMerge()
    {
        Destroy(gameObject, 0.25f);
    }

    public void StopPhysic()
    {
        _rb.bodyType = RigidbodyType2D.Static;
    }

    public void ContinuePhysic()
    {
        _rb.bodyType = RigidbodyType2D.Dynamic;
    }

    public List<int> FindObjectsInRadius(int layerMask)
    {
        List<int> foundObjects = new List<int>();

        // Tính bán kính dựa trên kích thước của Collider2D
        float radius = extents.magnitude;

        // Tìm tất cả các collider trong bán kính
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, layerMask);
        //lọc tìm tag obstacle
        if (colliders.Length <= 0) return null;
        else
        {
            foreach (var collider in colliders)
            {
                if (collider.CompareTag("LevelObstacle"))
                {
                    foundObjects.Add(collider.gameObject.GetComponent<IceController>().id);
                }
            }
        }

        return foundObjects;
    }

    public Vector2 GetRandomPointInPolygon()
    {
        PolygonCollider2D polygonCollider = this.GetComponent<PolygonCollider2D>();
        Bounds bounds = polygonCollider.bounds;

        while (true)
        {
            Vector2 randomPoint = new Vector2(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y)
            );

            // Kiểm tra xem điểm có nằm trong polygon không
            if (polygonCollider.OverlapPoint(randomPoint))
            {
                return randomPoint;
            }
        }
    }

    IArchitecture IBelongToArchitecture.GetArchitecture()
    {
        return BlueJellyClone.Interface;
    }
}
