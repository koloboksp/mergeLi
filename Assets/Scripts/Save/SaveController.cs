using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
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

public class SaveController
{
    const string PlayerSettingsFileName = "playerSettings";
    const string PlayerDataFileName = "playerData";
    const string PlayerLastSessionDataFileName = "playerLastSessionData";
    
    private Progress _progress = new Progress();
    private SessionProgress _lastSessionProgress = null;

    private SaveSettings _saveSettings;

    public SaveSettings SaveSettings => _saveSettings;
    
    public SaveController()
    {
        _saveSettings = new SaveSettings(this, PlayerSettingsFileName);
    }
    
    public void AddCoins(int amount)
    {
        _progress.Coins += amount;
        SaveData();
    }
    
    public int GetAvailableCoins()
    {
        return _progress.Coins;
    }
    
    public void ConsumeCoins(int count)
    {
        _progress.Coins -= count;
        SaveData();
    }

    
    public bool IsCastleCompleted(string id)
    {
        return _progress.CompletedCastles.Contains(id);
    }
   
    public void MarkCastleCompleted(string id)
    {
        if (_progress.CompletedCastles.Contains(id)) return;
        
        _progress.CompletedCastles.Add(id);
        SaveData();
    }
    
    public void SetBestSessionScore(int score)
    {
        _progress.BestSessionScore = score;
        
        SaveData();
    }

    private void SaveData()
    {
        try
        {
            string data = JsonUtility.ToJson(_progress);
            var path = Path.Combine(Application.persistentDataPath, PlayerDataFileName);
            File.WriteAllText(path, data);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
    
    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _saveSettings.InitializeAsync(cancellationToken);
            
            try
            {
                var path = Path.Combine(Application.persistentDataPath, PlayerDataFileName);
                var loadedFile = await File.ReadAllTextAsync(path, cancellationToken);
                var loadedProgress = JsonUtility.FromJson<Progress>(loadedFile);
                _progress = loadedProgress;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
           
            try
            {
                var path = Path.Combine(Application.persistentDataPath, PlayerLastSessionDataFileName);
                var loadedFile = await File.ReadAllTextAsync(path, cancellationToken);
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
        var path = Path.Combine(Application.persistentDataPath, PlayerLastSessionDataFileName);
        File.WriteAllText(path, data);
    }

    public SessionProgress LastSessionProgress => _lastSessionProgress;
   
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
        
        SaveData();
    }


    public void Save<T>(T data, string fileName)
    {
        try
        {
            var sData = JsonUtility.ToJson(data);
            var path = Path.Combine(Application.persistentDataPath, fileName);
            File.WriteAllText(path, sData);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public async Task<T> LoadAsync<T>(string fileName, CancellationToken cancellationToken) where T : class
    {
        try
        {
            var path = Path.Combine(Application.persistentDataPath, fileName);
            var loadedFile = await File.ReadAllTextAsync(path, cancellationToken);
            return JsonUtility.FromJson<T>(loadedFile);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

        return null;
    }
}