using System.Threading;
using System.Threading.Tasks;

namespace Core.Tutorials
{
    public class HideUIHatsPanelTutorialStep : TutorialStep
    {
        protected override async Task<bool> InnerExecuteAsync(CancellationToken cancellationToken)
        { 
            var panel = ApplicationController.Instance.UIPanelController.GetPanel<UIHatsPanel>();
            ApplicationController.Instance.UIPanelController.PopScreen(panel);

            return true;
        }
    }
}