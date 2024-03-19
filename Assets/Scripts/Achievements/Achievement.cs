using Core;
using UnityEngine;

public class Achievement : MonoBehaviour
{
    private GameProcessor _gameProcessor;

    public string Id => gameObject.name;
    public GameProcessor GameProcessor => _gameProcessor;

    protected virtual void InnerCheck()
    {
    }

    public virtual void SetData(GameProcessor gameProcessor)
    {
        _gameProcessor = gameProcessor;
    }

    protected void Unlock()
    {
        _ = ApplicationController.Instance.ISocialService.UnlockAchievement(Id, Application.exitCancellationToken);
    }
}