using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Tutorials
{
    public class HideFocuserModule : ModuleTutorialStep
    {
        [SerializeField] private bool _smooth;

        public override async Task OnExecuteAsync(TutorialStep step, CancellationToken cancellationToken)
        {
            await step.Tutorial.Controller.Focuser.HideAsync(_smooth, cancellationToken);
        }
    }
}