using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Utils;
using UnityEngine;

namespace Core.Tutorials
{
    public class ShowBallsEyesTutorialStep : TutorialStep, IClickOnSomething
    {
        [SerializeField] public Vector3Int[] _ballPositions;
        
        protected override async Task<bool> InnerExecuteAsync(CancellationToken cancellationToken)
        {
            var balls = Tutorial.Controller.GameProcessor.GetBalls(_ballPositions);
            foreach (var ball in balls)
                ball.View.ShowEyes(true);
            
            return true;
        }
    }
}