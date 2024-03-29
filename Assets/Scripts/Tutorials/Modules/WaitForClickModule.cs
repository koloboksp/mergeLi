﻿using System.Threading;
using System.Threading.Tasks;

namespace Core.Tutorials
{
    public class WaitForClickModule : ModuleTutorialStep
    {
        public override async Task OnExecuteAsync(TutorialStep step, CancellationToken cancellationToken)
        {
            await step.Tutorial.Controller.Focuser.WaitForClick(cancellationToken);
        }
    }
}