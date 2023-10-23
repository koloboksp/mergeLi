using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Tutorials
{
    public class DestroyActiveCastleTutorialStep : TutorialStep
    {
        protected override async Task<bool> InnerExecute(CancellationToken cancellationToken)
        {
            var activeCastle = Tutorial.Controller.GameProcessor.CastleSelector.ActiveCastle;
            await activeCastle.DestroyCastle(cancellationToken);

            return true;
        }
    }
}