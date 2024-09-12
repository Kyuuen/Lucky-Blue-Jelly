using DG.Tweening;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using ZBase.UnityScreenNavigator.Core.Activities;

public class DragToMoveUI : ZBase.UnityScreenNavigator.Core.Activities.Activity, IController
{
    [SerializeField] private Transform _startPoint;
    [SerializeField] private Transform _endPoint;
    [SerializeField] private GameObject _pointHand;

    private GameObject pointHand;

    void Awake()
    {

        Vector2 startPoint, endPoint;
        startPoint = Camera.main.ScreenToWorldPoint(_startPoint.position);
        endPoint = Camera.main.ScreenToWorldPoint(_endPoint.position);

        pointHand = Instantiate(_pointHand, startPoint, Quaternion.identity);
        pointHand.GetComponent<Rigidbody2D>().DOMove(endPoint, 2f)
            .SetLoops(-1, LoopType.Restart)
            .OnComplete(() => pointHand.transform.position = startPoint);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Destroy(pointHand);
            ActivityContainer.Find(ContainerKey.Activities).Hide(ResourceKeys.DragToMoveUIPrefab(), false);
        }
    }

    IArchitecture IBelongToArchitecture.GetArchitecture()
    {
        return BlueJellyClone.Interface;
    }
}
