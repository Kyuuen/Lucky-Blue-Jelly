using UnityEngine;
using QFramework;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class HammerAnimation : MonoBehaviour, IController
{
    Vector3 offset = new Vector3(0.2f, 0,0);

    IScoreSystem _scoreSystem;

    void Awake()
    {
        _scoreSystem = this.GetSystem<IScoreSystem>();
    }

    public void PlayAnimation(Transform bubble, int iD)
    {
        this.SendCommand(new BoosterInactivateCommand
        {
            _boosterType = 0,
            _isPopupOn = false
        }) ;
        gameObject.GetComponent<Rigidbody2D>().DOMove(bubble.position + offset, 0.5f).OnComplete(() =>
        {
            _scoreSystem.ChangeScore(iD);
            bubble.gameObject.GetComponent<BubbleController>().IsDone(0);
            GetComponent<Animator>().SetBool("moveDone", true);
            Destroy(gameObject, 2.5f);
        });
    }

    IArchitecture IBelongToArchitecture.GetArchitecture()
    {
        return BlueJellyClone.Interface;
    }
}
