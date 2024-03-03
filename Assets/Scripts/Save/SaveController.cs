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
    
    private SessionProgress _lastSessionProgress = null;

    private readonly SaveSettings _saveSettings;
    private readonly SaveProgress _saveProgress;
    
    public SaveSettings SaveSettings => _saveSettings;
    public SaveProgress SaveProgress => _saveProgress;

    public SessionProgress LastSessionProgress => _lastSessionProgress;
    
    public SaveController()
    {
        _saveSettings = new SaveSettings(this, PlayerSettingsFileName);
        _saveProgress = new SaveProgress(this, PlayerDataFileName);
    }
    
    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _saveSettings.InitializeAsync(cancellationToken);
            await _saveProgress.InitializeAsync(cancellationToken);
            
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
            _saveProgress.Clear();
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

    public void SaveSessionProgress(ISessionProgressHolder sessionProgressHolder)
    {
        var sessionProgress = new SessionProgress();

        sessionProgress.Score = sessionProgressHolder.GetScore();
        
        var castle = sessionProgressHolder.GetCastle();
        if (castle.Completed)
        {
            _saveProgress.MarkCastleCompleted(castle.Id);
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
    
    internal bool Save<T>(T data, string fileName)
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
            return false;
        }
        
        return true;
    }
    
    internal async Task<bool> SaveAsync<T>(T data, string fileName)
    {
        try
        {
            var sData = JsonUtility.ToJson(data);
            var path = Path.Combine(Application.persistentDataPath, fileName);
            await File.WriteAllTextAsync(path, sData);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return false;
        }
        
        return true;
    }
    
    internal void Clear(string fileName)
    {
        try
        {
            File.Delete(fileName);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    internal async Task<T> LoadAsync<T>(string fileName, CancellationToken cancellationToken) where T : class
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