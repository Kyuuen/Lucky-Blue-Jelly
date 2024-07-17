using QFramework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameController : MonoBehaviour, IController
{
    [SerializeField] private GameObject[] mNodes;
    [SerializeField] private GameObject mNodeContainer;
    private void Awake()
    {
        this.RegisterEvent<GameStartEvent>(e =>
        {
            mNodeContainer.SetActive(true);
            foreach (GameObject node in mNodes)
            {
                node.gameObject.SetActive(true);
            }
        }).UnRegisterWhenGameObjectDestroyed(gameObject);

        this.RegisterEvent<OnCountDownEvent>(e => {
            mNodeContainer.SetActive(false);
            Debug.Log("You lose!");
        }).UnRegisterWhenGameObjectDestroyed(gameObject);

        this.RegisterEvent<GamePassEvent>(e =>
        {
            mNodeContainer.SetActive(false);
            Debug.Log("You win!");
        }).UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    IArchitecture IBelongToArchitecture.GetArchitecture()
    {
        return TappingGame.Interface;
    }
}
