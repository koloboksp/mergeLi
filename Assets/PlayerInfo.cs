using System;
using System.Collections.Generic;
using System.IO;
using Core;
using Core.Goals;
using Core.Steps.CustomOperations;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

public interface ISessionProgressHolder
{
    ICastle GetCastle();
    IField GetField();
    IEnumerable<IBuff> GetBuffs();
    int GetScore();

    string GetFirstUncompletedCastle();
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
    
    public void AddCoins(int amount)
    {
        _progress.Coins += amount;
        Save();
        
        OnCoinsChanged?.Invoke();
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

    
    public bool IsCastleCompleted(string id)
    {
        return _progress.CompletedCastles.Contains(id);
    }
   
    public void MarkCastleCompleted(string id)
    {
        if (_progress.CompletedCastles.Contains(id)) return;
        
        _progress.CompletedCastles.Add(id);
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

        sessionProgress.Score = sessionProgressHolder.GetScore();

        
        var castle = sessionProgressHolder.GetCastle();
        if (castle.Completed)
        {
            MarkCastleCompleted(castle.Id);
            sessionProgress.Castle = new SessionCastleProgress()
            {
                Id = sessionProgressHolder.GetFirstUncompletedCastle(),
                Points = 0,
            };
        }
        else
        {
            sessionProgress.Castle = new SessionCastleProgress()
            {
                Id = castle.Id,
                Points = castle.GetPoints(),
            };
        }
       

        sessionProgress.Field = new SessionFieldProgress();
        foreach (var ball in sessionProgressHolder.GetField().GetAll<IBall>())
        {
            var ballProgress = new SessionBallProgress()
            {
                GridPosition = ball.IntGridPosition,
                Points = ball.Points,
            };
            sessionProgress.Field.Balls.Add(ballProgress);
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
    
    public bool IsTutorialComplete(string tutorialId)
    {
        var tutorialProgress = _progress.Tutorials.Find(i => string.Equals(i.Id, tutorialId, StringComparison.Ordinal));
        if (tutorialProgress != null)
            return tutorialProgress.IsComplted;

        return false;
    }

    public void CompleteTutorial(string tutorialId)
    {
        var tutorialProgress = _progress.Tutorials.Find(i => string.Equals(i.Id, tutorialId, StringComparison.Ordinal));
        if (tutorialProgress != null)
            tutorialProgress.IsComplted = true;
        else
        {
            _progress.Tutorials.Add(
                new TutorialProgress()
                {
                    Id = tutorialId,
                    IsComplted = true,
                });
        }
        
        Save();
    }
}

[Serializable]
public class Progress
{
    public List<string> CompletedCastles = new List<string>();
    public int BestSessionScore;
    public int Coins;
    public List<TutorialProgress> Tutorials = new();
}

[Serializable]
public class SessionProgress
{
    public SessionFieldProgress Field;
    public SessionCastleProgress Castle;
    public List<SessionBuffProgress> Buffs = new();
    public int Score;
       
    public bool IsValid()
    {
        if (string.IsNullOrEmpty(Castle.Id))
            return false;

        return true;
    }
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

[Serializable]
public class SessionCastleProgress
{ 
    public string Id;
    public int Points;
}

[Serializable]
public class TutorialProgress
{
    public string Id;
    public bool IsComplted;
}
