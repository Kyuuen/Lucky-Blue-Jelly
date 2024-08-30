using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStorage : IUtility
{
    void SaveInt(string key, int value);
    int LoadInt(string key, int defaultValue = 0);

    void SaveBool(string key, bool value);

    bool LoadBool(string key, bool defaultValue = true);
}

public class PlayerPrefsStorage : IStorage
{
    public void SaveInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
    }

    public int LoadInt(string key, int defaultValue = 0)
    {
        return PlayerPrefs.GetInt(key, defaultValue);
    }

    public void SaveBool(string key, bool value)
    {
        int intValue = value ? 1 : 0;
        PlayerPrefs.SetInt(key, intValue);
    }

    public bool LoadBool(string key, bool defaultValue = true)
    {
        int intDefaultValue = defaultValue ? 1 : 0;
        int intValue = PlayerPrefs.GetInt(key, intDefaultValue);
        return intValue != 0;
    }
}
