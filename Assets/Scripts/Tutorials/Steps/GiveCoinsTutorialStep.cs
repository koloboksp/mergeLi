using System.Threading;
using System.Threading.Tasks;

namespace Core.Tutorials
{
    public class GiveCoinsTutorialStep : TutorialStep
    {
        protected override async Task<bool> InnerExecuteAsync(CancellationToken cancellationToken)
        {
            var activeCastle = Tutorial.Controller.GameProcessor.CastleSelector.ActiveCastle;
           
            await Tutorial.Controller.GameProcessor.GiveCoinsEffect.Show(
                activeCastle.CoinsAfterComplete, 
                Tutorial.Controller.Dialog.Speaker.IconRoot.transform, 
                cancellationToken);
            
            return true;
        }
    }
}