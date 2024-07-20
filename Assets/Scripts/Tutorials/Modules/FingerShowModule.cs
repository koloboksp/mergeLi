using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Tutorials
{
    public class FingerShowModule : ModuleTutorialStep
    {
        [SerializeField] private FingerOrientation _fingerOrientation;
        [SerializeField] private FingerStyle _style = FingerStyle.Pointing;

        public override async Task OnExecuteAsync(TutorialStep step, CancellationToken cancellationToken)
        {
            if (step is IFocusedOnSomething focusedOnSomething)
            {
                step.Tutorial.Controller.Finger.Show(focusedOnSomething.GetFocusedRect(), _fingerOrientation, _style);
            }
        }
    }
}