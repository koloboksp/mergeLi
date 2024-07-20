using System.Threading;
using System.Threading.Tasks;
using UI.Panels;

namespace Core.Tutorials
{
    public class HideUICastlesLibraryPanelTutorialStep : TutorialStep
    {
        protected override async Task<bool> InnerExecuteAsync(CancellationToken cancellationToken)
        {
            var panel = ApplicationController.Instance.UIPanelController.GetPanel<UICastlesLibraryPanel>();
            ApplicationController.Instance.UIPanelController.PopScreen(panel);
            
            return true;
        }
    }
}