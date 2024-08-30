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
    }
    IArchitecture IBelongToArchitecture.GetArchitecture()
    {
        return BlueJellyClone.Interface;
    }
}
