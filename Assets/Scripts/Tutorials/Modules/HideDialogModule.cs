using System.Threading;
using System.Threading.Tasks;

namespace Core.Tutorials
{
    public class HideDialogModule : ModuleTutorialStep
    {
        public override async Task OnExecuteAsync(TutorialStep step, CancellationToken cancellationToken)
        {
            step.Tutorial.Controller.Dialog.Hide();
        }
    }
}