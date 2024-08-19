#if UNITY_WEBGL && !UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Save;
using UnityEngine;

public class UserDataManager : IStorage
{
    private const string WebGLSavePath = "save_files/";
    private const string WebGLPrefPath = "player_pref/";

    private const string allowedCharacters =
        "ABCDEFGHIJKLMNOPQRSTUVWXYZ abcdefghijklmnopqrstuvwxyz0123456789.";


    public static bool IsValidSaveFileName(string saveFile)
    {
        if (String.IsNullOrEmpty(saveFile))
        {
            Debug.LogWarning("Save File Name was null or empty");
            return false;
        }

        foreach (char c in saveFile)
        {
            if (!allowedCharacters.Contains(c.ToString()))
            {
                Debug.LogWarning("Save File Name does not use valid characters");
                return false;
            }
        }

        return true;
    }

    private static string GetPathForKey(string key)
    {
        return WebGLSavePath + key;
    }

    private static string GetPathForPreferenceKey(string key)
    {
        return WebGLPrefPath + key;
    }

    public static void SetString(string key, string data)
    {
        saveData(GetPathForKey(key), data);
    }

    public static void SetPreferenceString(string key, string data)
    {
        saveData(GetPathForPreferenceKey(key), data);
    }

    public static string GetString(string key, string defaultValue = "")
    {
        return loadData(GetPathForKey(key));
    }

    public static string GetPreferenceString(string key)
    {
        return loadData(GetPathForPreferenceKey(key));
    }

    public static bool HasKey(string key)
    {
        var data = loadData(GetPathForKey(key));
        return data != string.Empty;
    }

    public static bool HasPreferenceKey(string key)
    {
        var data = loadData(GetPathForPreferenceKey(key));
        return data != string.Empty;
    }

    public static void DeleteKey(string key)
    {
        deleteKey(GetPathForKey(key));
    }

    public static void DeleteAllSaves(string key)
    {
        deleteAllKeys(WebGLSavePath);
    }

    public static IEnumerable<string> ListAllSaveKeys()
    {
        string keysString = listAllKeys(WebGLSavePath);
        Debug.Log($"Unprocessed Array is: {keysString}");
        string[] keysArray = keysString.Split(',');
        Debug.Log($"KeyArray Length: {keysArray.Length} and contents: {string.Join(", ", keysArray)}");
        int i = 0;
        foreach (var key in keysArray)
        {
            if (IsValidSaveFileName(key)) {
                Debug.LogWarning($"Key {i}: {key}");
                i++;
                yield return key;
            }
            else
            {
                Debug.LogWarning($"Key {i}: {key} is not valid save file");
            }
        }
    }

    [DllImport("__Internal")]
    private static extern void saveData(string key, string data);

    [DllImport("__Internal")]
    private static extern string loadData(string key);

    [DllImport("__Internal")]
    private static extern string deleteKey(string key);

    [DllImport("__Internal")]
    private static extern string deleteAllKeys(string prefix);

    [DllImport("__Internal")]
    private static extern string listAllKeys(string prefix);

    public void Save(string fullPath, string sData)
    {
        SetString(fullPath, sData);
    }

    public Task SaveAsync(string fullPath, string sData)
    {
        SetString(fullPath, sData);
        return Task.CompletedTask;
    }

    public string Load(string fullPath)
    {
        return GetString(fullPath);
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }
}
#endif