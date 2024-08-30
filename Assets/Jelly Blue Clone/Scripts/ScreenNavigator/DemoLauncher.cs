using UnityEngine;
using ZBase.UnityScreenNavigator.Core;
using Cysharp.Threading.Tasks;
using ZBase.UnityScreenNavigator.Core.Windows;
using ZBase.UnityScreenNavigator.Core.Screens;
using ZBase.UnityScreenNavigator.Core.Views;
using System.Resources;
using QFramework;
using ZBase.UnityScreenNavigator.Core.Activities;

public class DemoLauncher : UnityScreenNavigatorLauncher, IController
{
    public static WindowContainerManager ContainerManager { get; private set; }

    protected override void OnAwake()
    {
        ContainerManager = this;
    }

    protected override void OnPostCreateContainers()
    {
        OpenScene().Forget();
    }

    private async UniTaskVoid OpenScene()
    {
        var options = new ActivityOptions(ResourceKeys.LoadingScenePrefab(), true);
        ActivityContainer.Find(ContainerKey.Activities).Show(options);
        
    }

    IArchitecture IBelongToArchitecture.GetArchitecture()
    {
        return BlueJellyClone.Interface;
    }
}
