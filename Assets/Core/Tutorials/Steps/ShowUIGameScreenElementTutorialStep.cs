using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Tutorials
{
    public class ShowUIGameScreenElementTutorialStep : TutorialStep
    {
        [SerializeField] private bool _progressBar;
        [SerializeField] private bool _coins;
        protected override async Task<bool> InnerExecute(CancellationToken cancellationToken)
        {
            var gameScreen = ApplicationController.Instance.UIPanelController.GetPanel<UIGameScreen>();
            if(_progressBar)
                gameScreen.SetActiveElement(UIGameScreenElement.ProgressBar, true);
            if(_coins)
                gameScreen.SetActiveElement(UIGameScreenElement.Coins, true);

            return true;
        }
    }
}