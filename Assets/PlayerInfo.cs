using System;
using System.Collections.Generic;
using System.IO;
using Core;
using Core.Goals;
using Core.Steps.CustomOperations;
using UnityEngine;
using Object = UnityEngine.Object;

public interface ISessionProgressHolder
{
    IField GetField();
    IEnumerable<IBuff> GetBuffs();
}

public class PlayerInfo : MonoBehaviour
{
    const string PlayerDataFileName = "playerData";
    const string PlayerLastSessionDataFileName = "playerLastSessionData";
    [SerializeField] private DefaultMarket _market;
    [SerializeField] private PurchasesLibrary _purchasesLibrary;
    
    public event Action OnCoinsChanged;
    public event Action OnCastleChanged;
    
    private Progress _progress = new Progress();
    private SessionProgress _lastSessionProgress = null;
    
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
            try
            {
                var loadedFile = File.ReadAllText(PlayerDataFileName);
                var loadedProgress = JsonUtility.FromJson<Progress>(loadedFile);
                _progress = loadedProgress;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
           
            try
            {
                var loadedFile = File.ReadAllText(PlayerLastSessionDataFileName);
                var lastSessionProgress = JsonUtility.FromJson<SessionProgress>(loadedFile);
                _lastSessionProgress = lastSessionProgress;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
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
            ClearLastSessionProgress();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public void ClearLastSessionProgress()
    {
        try
        {
            _lastSessionProgress = null;
            File.Delete(PlayerLastSessionDataFileName);
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

    public void SaveSessionProgress(ISessionProgressHolder sessionProgressHolder)
    {
        var sessionProgress = new SessionProgress();

        sessionProgress.SessionField = new SessionFieldProgress();
        foreach (var ball in sessionProgressHolder.GetField().GetAll<IBall>())
        {
            var ballProgress = new SessionBallProgress()
            {
                GridPosition = ball.IntGridPosition,
                Points = ball.Points,
            };
            sessionProgress.SessionField.Balls.Add(ballProgress);
        }

        sessionProgress.Buffs = new List<SessionBuffProgress>();
        foreach (var buff in sessionProgressHolder.GetBuffs())
        {
            var buffProgress = new SessionBuffProgress
            {
                Id = buff.Id,
                RestCooldown = buff.GetRestCooldown()
            };
            sessionProgress.Buffs.Add(buffProgress);
        }
        
        string data = JsonUtility.ToJson(sessionProgress);
        File.WriteAllText(PlayerLastSessionDataFileName, data);
    }

    public SessionProgress GetLastSessionProgress()
    {
        return _lastSessionProgress;
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


[Serializable]
public class SessionProgress
{
    public SessionFieldProgress SessionField;
    public List<SessionBuffProgress> Buffs = new List<SessionBuffProgress>();
}

[Serializable]
public class SessionFieldProgress
{
    public List<SessionBallProgress> Balls = new List<SessionBallProgress>();
}

[Serializable]
public class SessionBallProgress
{
    public Vector3Int GridPosition;
    public int Points;
}

[Serializable]
public class SessionBuffProgress
{
    public string Id;
    public int RestCooldown;
}