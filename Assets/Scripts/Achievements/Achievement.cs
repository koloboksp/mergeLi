using System.Collections.Generic;
using Core;
using Core.Steps;
using Core.Steps.CustomOperations;
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

public class BallsCombinationAchievement : Achievement
{
    public override void SetData(GameProcessor gameProcessor)
    {
        base.SetData(gameProcessor);

        GameProcessor.OnStepCompleted += GameProcessor_OnStepCompleted;
    }

    protected virtual void OnCollapse(CollapseOperationData data)
    {
    }

    private void GameProcessor_OnStepCompleted(Step step, StepExecutionType stepExecutionType)
    {
        if (stepExecutionType == StepExecutionType.Redo)
        {
            var collapseOperationData = step.GetData<CollapseOperationData>();
            if (collapseOperationData != null)
                OnCollapse(collapseOperationData);
        }
    }

    public enum LineDirection
    {
        Vertical,
        Horizontal,
        Diagonal,
        Undefined
    }

    public static LineDirection GetLineDirection(List<(Vector3Int intPosition, int points)> line)
    {
        var h = new List<int>();
        var v = new List<int>();

        foreach (var ball in line)
        {
            if (!h.Contains(ball.intPosition.x))
                h.Add(ball.intPosition.x);
            if (!v.Contains(ball.intPosition.y))
                v.Add(ball.intPosition.y);
        }

        h.Sort();
        v.Sort();

        if (h.Count == 1 && v.Count == line.Count)
        {
            var lineInterrupted = false;
            for (var i = 0; i < v.Count - 1; i++)
                if (v[i] + 1 != v[i + 1])
                {
                    lineInterrupted = true;
                    break;
                }

            if (!lineInterrupted)
                return LineDirection.Vertical;
        }

        if (v.Count == 1 && h.Count == line.Count)
        {
            var lineInterrupted = false;
            for (var i = 0; i < h.Count - 1; i++)
                if (h[i] + 1 != h[i + 1])
                {
                    lineInterrupted = true;
                    break;
                }

            if (!lineInterrupted)
                return LineDirection.Horizontal;
        }

        if (v.Count == line.Count && h.Count == line.Count)
        {
            var lineInterrupted = false;
            for (var i = 0; i < v.Count - 1; i++)
                if (v[i] + 1 != v[i + 1])
                {
                    lineInterrupted = true;
                    break;
                }

            for (var i = 0; i < h.Count - 1; i++)
                if (h[i] + 1 != h[i + 1])
                {
                    lineInterrupted = true;
                    break;
                }

            if (!lineInterrupted)
                return LineDirection.Diagonal;
        }

        return LineDirection.Undefined;
    }
}