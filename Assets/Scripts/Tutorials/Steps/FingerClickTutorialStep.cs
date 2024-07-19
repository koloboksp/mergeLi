using System.Threading;
using System.Threading.Tasks;

namespace Core.Tutorials
{
    public class FingerClickTutorialStep : TutorialStep
    {
        protected override async Task<bool> InnerExecuteAsync(CancellationToken cancellationToken)
        {
            Tutorial.Controller.Finger.Click();

            return true;
        }
    }
}