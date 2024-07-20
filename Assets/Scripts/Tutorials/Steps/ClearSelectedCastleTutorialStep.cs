using System.Threading;
using System.Threading.Tasks;

namespace Core.Tutorials
{
    public class ClearSelectedCastleTutorialStep : TutorialStep
    {
        protected override async Task<bool> InnerExecuteAsync(CancellationToken cancellationToken)
        {
            var castleSelector = Tutorial.Controller.GameProcessor.CastleSelector;
            castleSelector.SelectActiveCastle("");
            return true;
        }
    }
}