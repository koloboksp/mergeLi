using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Gameplay;
using Core.Steps;
using UnityEngine;
using UnityEngine.EventSystems;


public interface INextBallsShower
{
    void Show();
    void Hide();
}

public class ShowNextBallsBuff : Buff, INextBallsShower
{
    private readonly List<Ball> _balls = new();
    
    public override string Id => "ShowNextBalls";
    
    protected override void Inner_OnStepCompleted(Step step)
    {
        if (RestCooldown <= 0)
            ClearBalls(); 
        else
            ShowNextBalls();
    }

    protected override void Inner_OnRestCooldownChanged()
    {
        base.Inner_OnRestCooldownChanged();
        
        if (RestCooldown <= 0)
            ClearBalls(); 
        else
            ShowNextBalls();
    }

    private void ClearBalls()
    {
        foreach (var ball in _balls)
            Destroy(ball.gameObject);
        
        _balls.Clear();
    }
    
    private void ShowNextBalls()
    {
        var nextBallsData = _gameProcessor.Scene.Field.NextBallsData;
        var ballsToRemove = _balls
            .Where(ball => nextBallsData
                .All(i => ball.IntGridPosition != i.GridPosition))
            .ToList();
        
        var ballsToAdd = nextBallsData
            .Where(ballDesc => _balls
                .All(ball => ball.IntGridPosition != ballDesc.GridPosition))
            .ToList();

        foreach (var ball in ballsToRemove)
        {
            _balls.Remove(ball);
            Destroy(ball.gameObject);
        }

        for (var ballDescI = 0; ballDescI < ballsToAdd.Count; ballDescI++)
        {
            var nextBallData = ballsToAdd[ballDescI];
            var ball = _gameProcessor.Scene.Field.PureCreateBall(
                nextBallData.GridPosition, 
                nextBallData.Points,
                nextBallData.HatName);
            ball.name += "_next";
            ball.View.ShowPoints(false);
            _balls.Add(ball);

            if (ballDescI == 0)
            {
                ball.transform.localScale = Vector3.one * 0.5f;
                ball.Transparency = 0.2f;
            }
            else
            {
                ball.transform.localScale = Vector3.one * 0.3f;
                ball.Transparency = 0.4f;
            }
        }
    }

    protected virtual bool UndoAvailable => true;
    protected virtual StepTag UndoStepTag => StepTag.UndoNextBalls;

    protected override void InnerProcessUsing(PointerEventData pointerEventData)
    {
        _gameProcessor.UseShowNextBallsBuff(Cost, this, this);
    }

    public void Show()
    {
        ClearBalls();
        ShowNextBalls();
    }

    public void Hide()
    {
        ClearBalls();
    }
}