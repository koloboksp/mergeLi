using System.Threading;
using System.Threading.Tasks;

namespace Core.Tutorials
{
    public class ShowUIGameScreenTutorialStep : TutorialStep
    {
        protected override async Task<bool> InnerExecuteAsync(CancellationToken cancellationToken)
        {
            var gameScreen = await ApplicationController.Instance.UIPanelController.PushPopupScreenAsync<UIGameScreen>(
                new UIGameScreenData() { GameProcessor = Tutorial.Controller.GameProcessor }, 
                cancellationToken);
            gameScreen.HideAllElements();
            
            return true;
        }
    }
}