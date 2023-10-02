using System.Threading;
using System.Threading.Tasks;

namespace Core.Tutorials
{
    public class CompleteTutorialStep : TutorialStep
    {
        protected override async Task<bool> InnerExecute(CancellationToken cancellationToken)
        {
            Tutorial.Complete();
            return true;
        }
    }
}