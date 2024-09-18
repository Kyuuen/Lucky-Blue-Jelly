using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using ZBase.UnityScreenNavigator.Core.Modals;
using TMPro;
using Newtonsoft.Json;

public class FireCrackerAnimation : MonoBehaviour, IController
{
    [SerializeField] private ParticleSystem _particleSystem;

    [SerializeField] private LayerMask _bubbleMask;
    [SerializeField] private LayerMask _iceMask;

    SpriteRenderer _spriteRenderer;
    Vector3 extents;
    void Awake()
    {
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        extents = _spriteRenderer.bounds.extents;
    }

    public void PlayAnimation(Vector2 mousePos)
    {
        var modalContainer = string.IsNullOrEmpty(ContainerKey.Modals)
            ? ModalContainer.Of(transform)
            : ModalContainer.Find(ContainerKey.Modals);
        modalContainer.Pop(false);

        this.SendCommand(new BoosterInactivateCommand
        {
            _boosterType = 2,
            _isPopupOn = false
        });

        gameObject.GetComponent<Rigidbody2D>().DOMove(mousePos, 1f).SetEase(Ease.OutSine).OnComplete(() =>
        {
            this.SendCommand(new FirecrackerDestroyCommand
            {
                _bubbleIds = FindBubbleInRadius(_bubbleMask),
                _iceIds = FindIceInRadius(_iceMask),
            });

            GetComponent<SpriteRenderer>().enabled = false;
            _particleSystem.Play();
            Destroy(gameObject, 1f);
        });
    }

    public List<int> FindIceInRadius(int layerMask)
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
                if (collider.gameObject.GetComponent<IceController>().id != 10000)
                {
                    foundObjects.Add(collider.gameObject.GetComponent<IceController>().id);
                }
            }
        }
        return foundObjects;
    }

    public List<int> FindBubbleInRadius(int layerMask)
    {
        List<int> foundObjects = new List<int>();

        // Tính bán kính dựa trên kích thước của Collider2D
        float radius = extents.magnitude;

        // Tìm tất cả các collider trong bán kính
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, layerMask);
        List<string> colliderName = new();
        foreach (var collider in colliders)
        {
            colliderName.Add(collider.gameObject.name);
        }
        //lọc tìm tag obstacle
        if (colliders.Length <= 0) return null;
        else
        {
            foreach (var collider in colliders)
            {
                if (collider.gameObject.GetComponent<BubbleController>().iD != 100000 && !collider.isTrigger)
                {
                    foundObjects.Add(collider.gameObject.GetComponent<BubbleController>().iD);
                }
            }
        }
        return foundObjects;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, extents.magnitude);
    }

    IArchitecture IBelongToArchitecture.GetArchitecture()
    {
        return BlueJellyClone.Interface;
    }
}
