using System.Threading;
using System.Threading.Tasks;

namespace Core.Tutorials
{
    public class PlayCastleBuildingCompleteTutorialStep : TutorialStep
    {
        protected override async Task<bool> InnerExecuteAsync(CancellationToken cancellationToken)
        {
            var activeCastle = Tutorial.Controller.GameProcessor.CastleSelector.ActiveCastle;
            await activeCastle.PlayBuildingComplete();
            return true;
        }
    }
}