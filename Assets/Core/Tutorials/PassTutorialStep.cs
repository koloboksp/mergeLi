using System.Threading;
using System.Threading.Tasks;

namespace Core.Tutorials
{
    public class PassTutorialStep : TutorialStep
    {
        protected override async Task<bool> InnerExecute(CancellationToken cancellationToken)
        {
            return true;
        }
    }
}