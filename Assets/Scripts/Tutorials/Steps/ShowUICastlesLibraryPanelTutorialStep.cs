using System.Threading;
using System.Threading.Tasks;
using UI.Panels;
using UnityEngine;

namespace Core.Tutorials
{
    public class ShowUICastlesLibraryPanelTutorialStep : TutorialStep
    {
        protected override async Task<bool> InnerExecuteAsync(CancellationToken cancellationToken)
        {
            var gameProcessor = Tutorial.Controller.GameProcessor;
            
            var data = new UICastleLibraryPanelData
            {
                Selected = gameProcessor.CastleSelector.Library.Castles[0].Id,
                Castles = gameProcessor.CastleSelector.Library.Castles,
                GameProcessor = gameProcessor
            };
            _ = ApplicationController.Instance.UIPanelController.PushPopupScreenAsync<UICastlesLibraryPanel>(
                data,
                Application.exitCancellationToken);
            
            return true;
        }
    }
}