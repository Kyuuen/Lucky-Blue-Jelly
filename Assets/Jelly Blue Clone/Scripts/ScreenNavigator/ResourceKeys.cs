using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class ResourceKeys
{
    private const string InGameUI = "InGame";
    private const string LevelCompleted = "LevelCompletedUI";
    private const string LevelFail = "LevelFailUI";
    private const string SettingUI = "SettingUI";
    private const string MenuSettingUI = "MenuSettingUI";

    private const string Level = "Level{0}";

    private const string JellySpritePath = "Game Items/Game-Items_{0}";
    private const string IceSpritePath = "Ice/ice{0}";

    private const string LoadingScene = "LoadingScene";

    private const string DragToMoveUI = "DragToMoveUI";

    private const string MenuScene = "MenuScene";

    private const string NextLevelUI = "NextLevelUI";

    private const string BoosterUI = "Booster_{0}";

    private const string BoosterAni = "BoosterAni_{0}";

    private const string BunchOfCoin = "BunchOfCoin";

    private const string HammerPopupPrefab = "HammerBoosterPopup";
    private const string StrawberryPopupPrefab = "StrawberryBoosterPopup";
    private const string FirecrackerPopupPrefab = "FirecrackerBoosterPopup";

    public static string GameUIPrefab()
    {
        return InGameUI;
    }

    public static string LevelCompletedPrefab()
    {
        return LevelCompleted;
    }

    public static string LevelFailPrefab()
    {
        return LevelFail;
    }

    public static string CurrentLevelPrefab(int i)
    {
        return string.Format(Level, i);
    }

    public static string JellySpriteSource(int type)
    {
        return string.Format(JellySpritePath, type);
    }

    public static string IceSpriteSource(int status)
    {
        return string.Format(IceSpritePath, status);
    }

    public static string SettingUIPrefab()
    {
        return SettingUI;
    }

    public static string LoadingScenePrefab()
    {
        return LoadingScene;
    }

    public static string MenuSettingUIPrefab()
    {
        return MenuSettingUI;
    }

    public static string MenuScenePrefab()
    {
        return MenuScene;
    }

    public static string NextLevelUIPrefab()
    {
        return NextLevelUI;
    }

    public static string BoosterUIPrefab(int status)
    {
        return string.Format(BoosterUI, status);
    }

    public static string BoosterAniPrefab(int status)
    {
        return string.Format(BoosterAni, status);
    }

    public static string BunchOfCoinPrefab()
    {
        return BunchOfCoin;
    }

    public static string DragToMoveUIPrefab()
    {
        return DragToMoveUI;
    }

    public static string HammerBoosterPopupPrefab()
    {
        return HammerPopupPrefab;
    }
    public static string StrawberryBoosterPopupPrefab()
    {
        return StrawberryPopupPrefab;
    }
    public static string FirecrackerBoosterPopupPrefab()
    {
        return FirecrackerPopupPrefab;
    }

}
