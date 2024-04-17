using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Tutorials
{
    public class FocuserModule : ModuleTutorialStep
    {
        [SerializeField] private bool _smooth;
       
        public override async Task OnExecuteAsync(TutorialStep step, CancellationToken cancellationToken)
        {
            step.Tutorial.Controller.Focuser.gameObject.SetActive(true);
            if (step is IFocusedOnSomething focusedOnSomething)
            {
                await step.Tutorial.Controller.Focuser.FocusOnAsync(focusedOnSomething.GetFocusedRect(), _smooth, cancellationToken);
            }
            else
            {
                await step.Tutorial.Controller.Focuser.UnfocusOnAsync(_smooth, cancellationToken);
            }
        }

        public override async Task OnCompleteAsync(TutorialStep step, CancellationToken cancellationToken)
        {
            
        }

        public override void OnUpdate(TutorialStep step)
        {
            if (step is IFocusedOnSomething focusedOnSomething)
            {
                step.Tutorial.Controller.Focuser.ForceFocusOn(focusedOnSomething.GetFocusedRect());
            }
        }
    }
}