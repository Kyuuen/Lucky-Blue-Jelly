using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

public interface IPlayerPrefModel : IModel
{
    BindableProperty<int> Gold {  get; set; }
    BindableProperty<int> CurrentLevel { get; set; }
    BindableProperty<int> CurrentLevelToLoad { get; set; }

    BindableProperty<bool> SoundAvailable { get; set; }
    BindableProperty<bool> VibrateAvailable { get; set; }

    BindableProperty<int> UseOfHammer { get; set; }
    BindableProperty<int> UseOfStrawberry { get; set; }
    BindableProperty<int> UseOfFirecracker { get; set; }

    public void Reset();
    public void ResetBooster();
    public void SetToTest(int currentLevel);
    
    public void NextLevel();
}

public class PlayerPrefModel : AbstractModel, IPlayerPrefModel
{
    public BindableProperty<int> Gold {  set; get; } = new BindableProperty<int>()
    {
        Value = 0
    };

    public BindableProperty<int> CurrentLevel { set; get; } = new BindableProperty<int>()
    {
        Value = 0
    };

    public BindableProperty<int> CurrentLevelToLoad { get; set; } = new BindableProperty<int>()
    {
        Value = 0
    };

    public BindableProperty<bool> SoundAvailable { get; set; } = new BindableProperty<bool>()
    {
        Value = true
    };
    public  BindableProperty<bool> VibrateAvailable { get; set; } = new BindableProperty<bool>()
    {
        Value = true
    };

    public BindableProperty<int> UseOfHammer { set; get; } = new BindableProperty<int>()
    {
        Value = 0
    };
    public BindableProperty<int> UseOfStrawberry { set; get; } = new BindableProperty<int>()
    {
        Value = 0
    };
    public BindableProperty<int> UseOfFirecracker { set; get; } = new BindableProperty<int>()
    {
        Value = 0
    };

    private IStorage storage;
    protected override void OnInit()
    {
        storage = this.GetUtility<IStorage>();

        CurrentLevel.Value = storage.LoadInt(nameof(CurrentLevel), 1);
        CurrentLevelToLoad.Value = storage.LoadInt(nameof(CurrentLevelToLoad), 1);
        Gold.Value = storage.LoadInt(nameof(Gold), 0);
        SoundAvailable.Value = storage.LoadBool(nameof(SoundAvailable), true);
        VibrateAvailable.Value = storage.LoadBool(nameof (VibrateAvailable), true);
        UseOfHammer.Value = storage.LoadInt(nameof(UseOfHammer), 0);
        UseOfStrawberry.Value = storage.LoadInt(nameof(UseOfStrawberry), 0);
        UseOfFirecracker.Value = storage.LoadInt(nameof(UseOfFirecracker), 0);

        CurrentLevel.Register(v => storage.SaveInt(nameof(CurrentLevel), v));
        CurrentLevelToLoad.Register(v => storage.SaveInt(nameof(CurrentLevelToLoad), v));
        Gold.Register(v=>storage.SaveInt(nameof(Gold), v));
        SoundAvailable.Register(v => storage.SaveBool(nameof(SoundAvailable), v));
        VibrateAvailable.Register(v => storage.SaveBool(nameof (VibrateAvailable), v));

        UseOfHammer.Register(v => storage.SaveInt(nameof(UseOfHammer), v));
        UseOfStrawberry.Register(v => storage.SaveInt(nameof(UseOfStrawberry), v));
        UseOfFirecracker.Register(v => storage.SaveInt(nameof(UseOfFirecracker), v));

        CurrentLevel.Register(GetRandomBooosterUse);

    }

    public void Reset()
    {
        Gold.Value = 0;
        CurrentLevel.Value = 1;
    }

    public void ResetBooster()
    {
        UseOfHammer.Value = 0;
        UseOfStrawberry.Value = 0;
        UseOfFirecracker.Value = 0;
    }

    public void NextLevel()
    {
        CurrentLevel.Value++;
    }

    public void SetToTest(int currentLevel)
    {
        CurrentLevel.Value = currentLevel;
    }

    void GetRandomBooosterUse(int currentLevel)
    {
        if(currentLevel % 3 == 0)
        {
            if (currentLevel < 2) return;
            else if(currentLevel >= 2 && currentLevel < 5)
            {
                UseOfHammer.Value++;
            }else if(currentLevel >= 5 && currentLevel < 7)
            {
                int option = Random.Range(0, 2);
                if(option == 0)
                {
                    UseOfHammer.Value++;
                }
                else
                {
                    UseOfStrawberry.Value++;
                }
            }else if(currentLevel > 7)
            {
                int option = Random.Range(0, 3);
                switch (option)
                {
                    case 0:
                        {
                            UseOfHammer.Value++;
                            break;
                        }
                    case 1:
                        {
                            UseOfStrawberry.Value++;
                            break;
                        }
                    case 2:
                        {
                            UseOfFirecracker.Value++;
                            break;
                        }
                }
            }
        }
    }
}
