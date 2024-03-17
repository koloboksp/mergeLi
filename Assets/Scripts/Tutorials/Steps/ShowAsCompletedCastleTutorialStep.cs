using System.Threading;
using System.Threading.Tasks;

namespace Core.Tutorials
{
    public class ShowAsCompletedCastleTutorialStep : TutorialStep
    {
        protected override async Task<bool> InnerExecuteAsync(CancellationToken cancellationToken)
        {
            var activeCastle = Tutorial.Controller.GameProcessor.CastleSelector.ActiveCastle;
            activeCastle.ShowAsCompleted();

            return true;
        }
    }
}