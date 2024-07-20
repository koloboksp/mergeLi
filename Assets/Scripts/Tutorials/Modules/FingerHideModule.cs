using System.Threading;
using System.Threading.Tasks;

namespace Core.Tutorials
{
    public class FingerHideModule : ModuleTutorialStep
    {
        public override async Task OnCompleteAsync(TutorialStep step, CancellationToken cancellationToken)
        {
            step.Tutorial.Controller.Finger.Hide();
        }
    }
}