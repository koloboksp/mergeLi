using System.Threading;
using System.Threading.Tasks;
using Core.Gameplay;
using UnityEngine;

namespace Core.Tutorials
{
    public class GenerateFieldTutorialStep : TutorialStep
    {
        [SerializeField] public BallDesc[] _balls;
        [SerializeField] public BallDesc[] _nextBalls;
        [SerializeField] public Field _field;
        
        protected override async Task<bool> InnerExecuteAsync(CancellationToken cancellationToken)
        {
            _field.AddBalls(_balls);
            _field.SetNextBalls(_nextBalls);

            return true;
        }
    }
}