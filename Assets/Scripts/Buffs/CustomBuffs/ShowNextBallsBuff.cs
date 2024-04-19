using System.Collections.Generic;
using Core;
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
        ClearBalls();
        if(RestCooldown != 0)
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
        for (var index = 0; index < nextBallsData.Count; index++)
        {
            var nextBallData = nextBallsData[index];
            var ball = _gameProcessor.Scene.Field.PureCreateBall(
                nextBallData.GridPosition, 
                nextBallData.Points,
                nextBallData.Hat);
            _balls.Add(ball);

            if (index == 0)
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