using System.Threading;
using System.Threading.Tasks;

namespace Core.Tutorials
{
    public class HideUIGameScreenTutorialStep : TutorialStep
    {
        protected override async Task<bool> InnerExecuteAsync(CancellationToken cancellationToken)
        {
            var screen = ApplicationController.Instance.UIPanelController.GetPanel<UIGameScreen>();
            ApplicationController.Instance.UIPanelController.PopScreen(screen);
            
            return true;
        }
    }
}