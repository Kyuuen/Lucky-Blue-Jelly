using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using Cysharp.Threading.Tasks;

public interface IGoldSystem : ISystem 
{
    public void GetExtraGold(int multiply);
    public void CalculateGold(List<int> id);
}

public class GoldSystem : AbstractSystem, IGoldSystem
{
    private List<int> goldPerSize = new List<int> { 5, 10, 15, 20 };

    private IGameSceneModel gameSceneModel;
    private IPlayerPrefModel prefModel;
    protected override void OnInit()
    {
        prefModel = this.GetModel<IPlayerPrefModel>();
        gameSceneModel = this.GetModel<IGameSceneModel>(); 
    }

    public void CalculateGold(List<int> id)
    {
        foreach(int i in id)
        {
            prefModel.Gold.Value += goldPerSize[gameSceneModel.Bubbles[i].Size];
        }
        this.SendEvent<PostLevelWinEvent>();
    }

    public void GetExtraGold(int multiply)
    {
        prefModel.Gold.Value += 50 * multiply;
    }

    
}
