using System.Threading;
using System.Threading.Tasks;
using Core.Gameplay;
using UnityEngine;

namespace Core.Tutorials
{
    public class FillFieldTutorialStep : TutorialStep
    {
        [SerializeField] public BallsMaskPointsHats _balls;
        [SerializeField] public BallsMaskPointsHats _nextBalls;
        [SerializeField] public Field _field;
        
        protected override async Task<bool> InnerExecuteAsync(CancellationToken cancellationToken)
        {
            _field.AddBalls(_balls.Balls);
            _field.SetNextBalls(_nextBalls.Balls);

            return true;
        }
    }
}