using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnNewBubbleSpawnCommand : AbstractCommand, ICommand
{
    IGameSceneModel gameSceneModel;
    ISpawnJellySystem spawnJellySystem;
    protected override void OnExecute()
    {
        gameSceneModel = this.GetModel<IGameSceneModel>();
        spawnJellySystem = this.GetSystem<ISpawnJellySystem>();
        BubbleInfo newBubble = new BubbleInfo();
        newBubble.GetNewBubbleInfo();
        spawnJellySystem.GetRandomJellyInfo(newBubble);
        newBubble.ID = gameSceneModel.BubbleIds;
        gameSceneModel.BubbleIds++;
        gameSceneModel.Bubbles.Add(newBubble);
    }
}
