using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class StrawberryAnimation : MonoBehaviour, IController
{

    public void PlayAnimation()
    {
        this.SendCommand(new BoosterInactivateCommand
        {
            _boosterType = 1,
            _isPopupOn = false
        });
        gameObject.GetComponent<Rigidbody2D>().DOMove(new Vector2(0, 0), 0.5f).OnComplete(() =>
        {
            GetComponent<Animator>().SetBool("moveDone", true);
            Destroy(gameObject, 2.73f);
        });
    }

    IArchitecture IBelongToArchitecture.GetArchitecture()
    {
        return BlueJellyClone.Interface;
    }
}
