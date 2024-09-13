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

    [SerializeField] GameObject preLoadImage;

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
        await UniTask.WaitForSeconds(5);
         var options = new ActivityOptions(ResourceKeys.LoadingScenePrefab(), true);
         await ActivityContainer.Find(ContainerKey.Activities).ShowAsync(options);
        Destroy(preLoadImage);
    }

    IArchitecture IBelongToArchitecture.GetArchitecture()
    {
        return BlueJellyClone.Interface;
    }
}
