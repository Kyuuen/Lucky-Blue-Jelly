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
            gameIsEnd = true;
        });
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!_gameSceneModel.IsDropping && !gameIsEnd && collision.gameObject.layer == LayerMask.NameToLayer("Clone"))
        {
            count -= Time.deltaTime;
            if(count <= 0)
            {
                this.SendCommand<GameOverCommand>();
                gameIsEnd = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!_gameSceneModel.IsDropping)
        {
            count = delayTime;
        }
    }

    IArchitecture IBelongToArchitecture.GetArchitecture()
    {
        return BlueJellyClone.Interface;
    }
}
