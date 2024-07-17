using QFramework;
using Unity.VisualScripting;
using UnityEngine;

public class NodeController : MonoBehaviour, IController
{
    private void OnMouseDown()
    {
        this.gameObject.SetActive(false);

        this.SendCommand<ClickNodeCommand>();
    }
    IArchitecture IBelongToArchitecture.GetArchitecture()
    {
        return TappingGame.Interface;
    }
}
