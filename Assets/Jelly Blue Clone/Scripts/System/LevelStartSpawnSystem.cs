using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using System.Drawing;
using System.Linq;

public interface ILevelStartSpawnSystem : ISystem
{
    public void SetBubbleToSpawn(BubbleConfig bubbleConfig);
}
public class LevelStartSpawnSystem : AbstractSystem, ILevelStartSpawnSystem
{
    private IGameSceneModel _gameSceneModel;
    private ISpawnJellySystem _jellySystem;
    protected override void OnInit()
    {
        _gameSceneModel = this.GetModel<IGameSceneModel>();
        _jellySystem = this.GetSystem<ISpawnJellySystem>();
    }

    public void SetBubbleToSpawn(BubbleConfig bubbleConfig)
    {
        BubbleInfo newBubble = new BubbleInfo();
        newBubble.SetNewBubbleInfo(bubbleConfig.size);
        newBubble.ID = _gameSceneModel.BubbleIds;
        _gameSceneModel.BubbleIds++;
        _jellySystem.SetJellyInfo(newBubble, bubbleConfig.numb,bubbleConfig.numColor, bubbleConfig.colorRate);
        _gameSceneModel.Bubbles.Add(newBubble);
    }
}
