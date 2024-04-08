using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Tutorials
{
    public class GenerateFieldTutorialStep : TutorialStep
    {
        [SerializeField] public List<BallDesc> _balls;
        [SerializeField] public List<BallDesc> _nextBalls;
        [SerializeField] public Field _field;
        
        protected override async Task<bool> InnerExecuteAsync(CancellationToken cancellationToken)
        {
            var ballsInfos = _balls.Select(i => new BallDesc(i.GridPosition, i.Points));
            _field.AddBalls(ballsInfos);
            var nextBallsInfos = _nextBalls.Select(i => (i.GridPosition, i.Points));
            _field.SetNextBalls(nextBallsInfos);

            return true;
        }
    }
}