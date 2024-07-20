using System.Threading;
using System.Threading.Tasks;

namespace Core.Tutorials
{
    public class ScrollUIHatsPanelTutorialStep : TutorialStep
    {
        protected override async Task<bool> InnerExecuteAsync(CancellationToken cancellationToken)
        { 
            var panel = ApplicationController.Instance.UIPanelController.GetPanel<UIHatsPanel>();
            panel.StartAutoScrollContent(0.1f);

            return true;
        }
    }
}