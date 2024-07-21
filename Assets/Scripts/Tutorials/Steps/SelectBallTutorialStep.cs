using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Gameplay;
using Core.Utils;
using UnityEngine;

namespace Core.Tutorials
{
    public class SelectBallTutorialStep : TutorialStep, IClickOnSomething
    {
        [SerializeField] public Vector3Int _ballPosition;
        
        protected override async Task<bool> InnerExecuteAsync(CancellationToken cancellationToken)
        {
            Tutorial.Controller.GameProcessor.SelectBall(_ballPosition);
            
            await AsyncExtensions.WaitForSecondsAsync(0.5f, cancellationToken);

            return true;
        }
    }
}