using System.Threading;
using System.Threading.Tasks;

namespace Core.Tutorials
{
    public class ShowUIGameScreenTutorialStep : TutorialStep
    {
        protected override async Task<bool> InnerExecute(CancellationToken cancellationToken)
        {
            var gameScreen = await ApplicationController.Instance.UIPanelController.PushPopupScreenAsync(
                typeof(UIGameScreen), 
                new UIGameScreenData() { GameProcessor = Tutorial.Controller.GameProcessor }, 
                cancellationToken) as UIGameScreen;
            gameScreen.HideAllElements();
            
            return true;
        }
    }
}