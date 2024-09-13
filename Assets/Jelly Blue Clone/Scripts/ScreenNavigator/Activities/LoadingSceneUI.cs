using Cysharp.Threading.Tasks;
using QFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZBase.UnityScreenNavigator.Core.Activities;
using ZBase.UnityScreenNavigator.Core.Screens;
using ZBase.UnityScreenNavigator.Core.Views;

public class LoadingSceneUI : ZBase.UnityScreenNavigator.Core.Activities.Activity, IController
{
    [SerializeField] private Slider loadBar;

    [SerializeField] private float minDelay = 0.1f; // Minimum time between updates
    [SerializeField] private float maxDelay = 0.5f; // Maximum time between updates
    [SerializeField] private float minIncrement = 0.01f; // Minimum progress increment
    [SerializeField] private float maxIncrement = 0.1f; // Maximum progress increment

    private bool isLoading = true;
    private IUISystem _uiSystem;
    private IPlayerPrefModel _prefModel;

    void Awake()
    {
        Application.targetFrameRate = 60;
        _prefModel = this.GetModel<IPlayerPrefModel>();
    }

    public override async UniTask Initialize(Memory<object> args)
    {
        _uiSystem = this.GetSystem<IUISystem>();
        _uiSystem.CameraResizer(Camera.main);
        RandomlyUpdateLoadBar().Forget();
    }

    private async UniTask RandomlyUpdateLoadBar()
    {
        var options = new ViewOptions(ResourceKeys.GameUIPrefab(), true);
        // Reset load bar at the start
        loadBar.value = 1f;

        while (isLoading)
        {
            // Wait for a random delay
            float delay = UnityEngine.Random.Range(minDelay, maxDelay);
            await UniTask.Delay((int)(delay * 1000)); // Convert to milliseconds

            // Increment the load bar by a random amount
            float increment = UnityEngine.Random.Range(minIncrement, maxIncrement);
            loadBar.value -= increment;

            // Clamp the value to make sure it doesn't exceed 1.0 (100%)
            loadBar.value = Mathf.Clamp01(loadBar.value);

            // If loadBar is full, stop the loading
            if (loadBar.value <= 0f)
            {
                isLoading = false;
                OnLoadComplete();
            }
        }
        await ScreenContainer.Find(ContainerKey.Screens).PushAsync(options);
    }

    void OnLoadComplete()
    {
        ActivityContainer.Find(ContainerKey.Activities).Hide(ResourceKeys.LoadingScenePrefab());
    }

    public override void DidExit(Memory<object> args)
    {
        if (_prefModel.CurrentLevel.Value > 1) return;
        var options = new ActivityOptions(ResourceKeys.DragToMoveUIPrefab(), true);
        ActivityContainer.Find(ContainerKey.Activities).Show(options);
    }

    IArchitecture IBelongToArchitecture.GetArchitecture()
    {
        return BlueJellyClone.Interface;
    }
}
