using System.Collections.Generic;
using Core;
using Core.Steps;
using UnityEngine;

public class ShowNextBallsBuff : Buff
{
    [SerializeField] private int _stepsCount = 3;
    
    private int _restSteps;
    private readonly List<Ball> _balls = new();
    
    
    protected override void InnerOnClick()
    {
        Available = false;
        
        _gameProcessor.OnStepCompleted += GameProcessor_OnStepCompleted;
        _restSteps = _stepsCount - 1;
        
        ClearBalls();
        ShowNextBalls();
    }

    private void GameProcessor_OnStepCompleted(Step step)
    {
        if (step.Tag == "Move"
            || step.Tag == "Merge")
        {
            ClearBalls();

            if (_restSteps != 0)
            {
                ShowNextBalls();
                _restSteps--;
            }
            else
            {
                _gameProcessor.OnStepCompleted -= GameProcessor_OnStepCompleted;
                Available = true;
            }
        }
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
            var ball = _gameProcessor.Scene.Field.PureCreateBall(nextBallData.intPosition, nextBallData.points);
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
}