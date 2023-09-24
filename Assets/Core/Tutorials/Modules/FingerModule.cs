using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Tutorials
{

    public interface IFocusedOnSomething
    {
        Rect GetFocusedRect();
    }
    
    public class FingerModule : ModuleTutorialStep
    {
        public override async Task OnExecute(TutorialStep step, CancellationToken cancellationToken)
        {
            var focusedOnSomething = step as IFocusedOnSomething;
            step.Tutorial.Controller.Finger.Show(focusedOnSomething.GetFocusedRect());
        }

        public override async Task OnComplete(TutorialStep step, CancellationToken cancellationToken)
        {
            step.Tutorial.Controller.Finger.Hide();
        }

        public override void OnUpdate(TutorialStep step, CancellationToken cancellationToken)
        {
            
        }
    }
}