using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using Unity.VisualScripting;

public class SpawnUIController : MonoBehaviour, IController
{
    void OnMouseDown()
    {
        this.SendCommand<SwitchBubbleCommand>();
        AudioManager.Instance.PlayClickSound("click1");
    }
    IArchitecture IBelongToArchitecture.GetArchitecture()
    {
        return BlueJellyClone.Interface;
    }
}
