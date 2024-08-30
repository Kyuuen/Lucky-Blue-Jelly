using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class BunchOfCoin : MonoBehaviour, IController
{
    [SerializeField] private List<GameObject> _coins;
    public async void PlayCoinCollectAnimation(Vector2 destination, float duration)
    {
        await UniTask.Delay(500);
        foreach (var coin in _coins)
        {
            Rigidbody2D rb = coin.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            
            rb.DOMove(destination, duration).OnComplete(() =>
            {
                Destroy(gameObject);
            });
        }
    }
    IArchitecture IBelongToArchitecture.GetArchitecture()
    {
        return BlueJellyClone.Interface;
    }
}
