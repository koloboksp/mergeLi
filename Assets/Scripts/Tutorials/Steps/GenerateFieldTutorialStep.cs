using System.Threading;
using System.Threading.Tasks;
using Core.Gameplay;
using UnityEngine;

namespace Core.Tutorials
{
    public class GenerateFieldTutorialStep : TutorialStep
    {
        [SerializeField] private BallDesc[] _balls;
        [SerializeField] private BallDesc[] _nextBalls;
        [SerializeField] private Field _field;
        [SerializeField] private BallsMaskPointsHats _ballsMask;
        [SerializeField] private BallsMaskPointsHats _nextBallsMask;

        protected override async Task<bool> InnerExecuteAsync(CancellationToken cancellationToken)
        {
            _field.AddBalls(_ballsMask.Balls);
            _field.SetNextBalls(_nextBallsMask.Balls);

            return true;
        }
    }
}