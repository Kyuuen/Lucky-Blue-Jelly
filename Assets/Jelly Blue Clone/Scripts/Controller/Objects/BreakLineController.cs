using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using Unity.VisualScripting;

public class BreakLineController : MonoBehaviour, IController
{
    private IGameSceneModel _gameSceneModel;

    float delayTime = 5f;
    float count;
    bool gameIsEnd = false;
    private void Awake()
    {
        _gameSceneModel = this.GetModel<IGameSceneModel>();

        count = delayTime;
        this.RegisterEvent<RestartLevelEvent>(e =>
        {
            gameIsEnd = false;
        }).UnRegisterWhenGameObjectDestroyed(gameObject);
        this.RegisterEvent<PlayOnEvent>(e =>
        {
            count = delayTime;
            gameIsEnd = false;
        }).UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!gameIsEnd && collision.gameObject.layer == LayerMask.NameToLayer("Clone"))
        {
            count -= Time.deltaTime;
            if (count <= 0)
            {
                this.SendCommand<GameOverCommand>();
                gameIsEnd = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //if (gameIsEnd) return;
        if (collision.tag == "Bubble" && !collision.isTrigger && !collision.GetComponent<BubbleController>()._firstHit && !collision.GetComponent<BubbleController>()._spawnEarly)
        {
            this.SendCommand<DroppedBubbleCommand>();
            collision.GetComponent<BubbleController>()._firstHit = true;
        }

        if (!_gameSceneModel.IsDropping && !collision.CompareTag("Jelly"))
        {
            count = delayTime;
        }
    }

    IArchitecture IBelongToArchitecture.GetArchitecture()
    {
        return BlueJellyClone.Interface;
    }
}
