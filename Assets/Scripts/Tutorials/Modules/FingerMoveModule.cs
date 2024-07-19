using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Tutorials
{
    public class FingerMoveModule : ModuleTutorialStep
    {
        [SerializeField] private FingerOrientation _fingerOrientation;
        
        public override async Task OnExecuteAsync(TutorialStep step, CancellationToken cancellationToken)
        {
            var focusedOnSomething = step as IFocusedOnSomething;
            step.Tutorial.Controller.Finger.Show(focusedOnSomething.GetFocusedRect(), _fingerOrientation, FingerStyle.Normal);
        }

        public override async Task OnCompleteAsync(TutorialStep step, CancellationToken cancellationToken)
        {
            step.Tutorial.Controller.Finger.Hide();
        }
    }
}