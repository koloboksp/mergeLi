using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Tutorials
{
    public class FocuserModule : ModuleTutorialStep
    {
        [SerializeField] private bool _smooth;
       
        public override async Task OnExecute(TutorialStep step, CancellationToken cancellationToken)
        {
            await UpdateFocus(step as IFocusedOnSomething, step.Tutorial.Controller, cancellationToken);
        }

        public override async Task OnComplete(TutorialStep step, CancellationToken cancellationToken)
        {
            
        }

        public override void OnUpdate(TutorialStep step, CancellationToken cancellationToken)
        {
            UpdateFocus(step as IFocusedOnSomething, step.Tutorial.Controller, cancellationToken);
        }

        async Task UpdateFocus(IFocusedOnSomething focusedOnSomething, TutorialController controller, CancellationToken cancellationToken)
        {
            controller.Focuser.gameObject.SetActive(true);
            
            if (focusedOnSomething != null)
                await controller.Focuser.FocusOnAsync(focusedOnSomething.GetFocusedRect(), _smooth, cancellationToken);
            else
                await controller.Focuser.FocusOnAsync(_smooth, cancellationToken);
        }
    }
}