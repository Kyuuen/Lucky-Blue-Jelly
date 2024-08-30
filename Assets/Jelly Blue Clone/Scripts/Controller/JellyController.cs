using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using Unity.VisualScripting;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using static IBoosterSystem;

public class JellyController : MonoBehaviour, IController
{
    public int Type;

    public Vector2 _destination;
    public float _duration;
    public bool isBeingCollected;
    private DistanceJoint2D _joint;
    private Rigidbody2D _rb;
    private CircleCollider2D _circleCollider;

    private bool _boosterIsOn;
    private int _boosterStatus;

    [SerializeField] private SpriteRenderer _spriteRenderer;
    void Awake()
    {
        _joint = gameObject.AddComponent<DistanceJoint2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rb = GetComponent<Rigidbody2D>();
        _circleCollider = GetComponent<CircleCollider2D>();
    }

    public void ActivateBooster()
    {
        _spriteRenderer.sortingLayerName = "UI";
        _spriteRenderer.sortingOrder = 2001;
    }

    public void DeactivateBooster()
    {
        _spriteRenderer.sortingLayerName = "Default";
        _spriteRenderer.sortingOrder = 2;
    }

    public void PlayMoveAnimate(GameObject bubble, float duration)
    {
        _circleCollider.enabled = false;
        _joint.enabled = false;
        _rb.drag = 1000;
        bubble.GetComponent<BubbleController>().StopPhysic();
        _rb.DOMove(bubble.transform.position, duration).OnComplete(() =>
        {
            ConnectToBubble(bubble.GetComponent<BubbleController>()._rb, bubble.GetComponent<BubbleController>().distance);
            transform.SetParent(bubble.transform, true);
            _circleCollider.enabled=true;
            _rb.drag = 5;
            bubble.GetComponent<BubbleController>().ContinuePhysic();
        });
    }

    IEnumerator Collected()
    {
        _rb.gravityScale = 0f;
        _circleCollider.enabled = false;
        yield return new WaitForSeconds(0.5f);
        //DisConnectToBubble();
        this.SendCommand(new OnJellyCollectCommand {
            jelliesPos = transform.position,
            type = Type
        });

        Destroy(gameObject);
    }


    private void Update()
    {
        if (isBeingCollected) StartCoroutine(Collected());
    }
    public void ConnectToBubble(Rigidbody2D rb, float distance)
    {
        _joint.enabled = true;
        _joint.connectedBody = rb;
        _joint.autoConfigureDistance = false;
        _joint.distance = distance;
        _joint.maxDistanceOnly = true;
    }

    public void DisConnectToBubble()
    {
        _joint.enabled = false;
        transform.SetParent(null);
    }

    IArchitecture IBelongToArchitecture.GetArchitecture()
    {
        return BlueJellyClone.Interface;
    }
}
