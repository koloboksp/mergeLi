using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Tutorials
{
    public class SelectBallTutorialStep : TutorialStep
    {
        [SerializeField] public Vector3Int _ballPosition;
        
        protected override async Task<bool> InnerExecute(CancellationToken cancellationToken)
        {
            Tutorial.Controller.GameProcessor.SelectBall(_ballPosition);
            var enumerable = Tutorial.Controller.GameProcessor.GetField().GetAll<Ball>();
            var first = enumerable.First();

            await AsyncExtensions.WaitForSecondsAsync(1.0f, cancellationToken);

            return true;
        }
    }
}