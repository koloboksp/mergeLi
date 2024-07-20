using System.Threading;
using System.Threading.Tasks;

namespace Core.Tutorials
{
    public class FingerModule : ModuleTutorialStep
    {
        public override async Task OnExecuteAsync(TutorialStep step, CancellationToken cancellationToken)
        {
            if (step is IClickOnSomething clickOnSomething)
            {
                step.Tutorial.Controller.Finger.Click();
            }
        }

        
        
        public override void OnUpdate(TutorialStep step)
        {
            if (step is IFocusedOnSomething focusedOnSomething)
            {
                step.Tutorial.Controller.Finger.ForceFocusOn(focusedOnSomething.GetFocusedRect());
            }
        }
    }
}