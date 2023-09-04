using System;
using System.Collections.Generic;
using System.IO;
using Core;
using Core.Goals;
using Core.Steps.CustomOperations;
using UnityEngine;
using Object = UnityEngine.Object;

public class PlayerInfo : MonoBehaviour
{
    const string PlayerDataFileName = "playerData";
    [SerializeField] private DefaultMarket _market;
    [SerializeField] private PurchasesLibrary _purchasesLibrary;
    
    public event Action OnCoinsChanged;
    public event Action OnCastleChanged;
    
    private Progress _progress = new Progress();
    
    private void Awake()
    {
        _market.OnBought += Market_OnBought;
    }

    private void Market_OnBought(bool result, string inAppId)
    {
        var purchaseItem = _purchasesLibrary.Items.Find(i => string.Equals(i.InAppId, inAppId, StringComparison.Ordinal));
        if (purchaseItem != null)
        {
            _progress.Coins += purchaseItem.CurrencyAmount;
            Save();
            
            OnCoinsChanged?.Invoke();
        }
    }

    public int GetAvailableCoins()
    {
        return _progress.Coins;
    }


    public void ConsumeCoins(int count)
    {
        _progress.Coins -= count;
        Save();
        OnCoinsChanged?.Invoke();
    }

    public string GetLastSelectedCastle()
    {
        return _progress.LastSelectedCastle;
    }

    public CastleProgress GetCastleProgress(string castleName)
    {
        return _progress.CastleProgresses.Find(i => i.Name == castleName);
    }

    public void SelectCastle(string castleName)
    {
        _progress.LastSelectedCastle = castleName;
        Save();
        
        OnCastleChanged?.Invoke();
    }

    public void SetCastleProgress(CastleProgress castleProgress)
    {
        var foundIndex = _progress.CastleProgresses.FindIndex(i => i.Name == castleProgress.Name);
        if (foundIndex < 0)
            _progress.CastleProgresses.Add(castleProgress);
        else
            _progress.CastleProgresses[foundIndex] = castleProgress;
        
        Save();
    }
    
    public void SetBestSessionScore(int score)
    {
        _progress.BestSessionScore = score;
        
        Save();
    }

    public void Save()
    {
        try
        {
            string data = JsonUtility.ToJson(_progress);
            File.WriteAllText(PlayerDataFileName, data);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
    
    public void Load()
    {
        try
        {
            var loadedFile = File.ReadAllText(PlayerDataFileName);
            var loadedProgress = JsonUtility.FromJson<Progress>(loadedFile);

            _progress = loadedProgress;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public void Clear()
    {
        try
        {
            _progress = new Progress();
            File.Delete(PlayerDataFileName);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public int GetBestSessionScore()
    {
        return _progress.BestSessionScore;
    }
}

[Serializable]
public class Progress
{
    public List<CastleProgress> CastleProgresses = new List<CastleProgress>();
    public int BestSessionScore;
    public string LastSelectedCastle;
    public int Coins;
}

[Serializable]
public class CastleProgress
{
    public string Name;
    public int SelectedPartIndex;
  
    public List<CastlePartProgress> Parts = new List<CastlePartProgress>();
    public bool IsCompleted;
}
    
[Serializable]
public class CastlePartProgress
{
    public Vector2Int GridPosition;
    public bool IsCompleted;
}
