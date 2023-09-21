using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Tutorials
{
    public class ShowUIGameScreenElementTutorialStep : TutorialStep
    {
        [SerializeField] private bool _progressBar;
        protected override async Task<bool> InnerExecute(CancellationToken cancellationToken)
        {
            var gameScreen = ApplicationController.Instance.UIPanelController.GetPanel<UIGameScreen>();
            gameScreen.SetActiveElement(UIGameScreenElement.ProgressBar, true);
            
            return true;
        }
    }
}