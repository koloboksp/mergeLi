using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Tutorials
{
    public class ShowUIGameScreenElementTutorialStep : TutorialStep
    {
        [SerializeField] private bool _progressBar;
        [SerializeField] private bool _coins;
        [SerializeField] private bool _buffs;
        [SerializeField] private bool _settingsBtn;
        [SerializeField] private bool _adsBtn;
        protected override async Task<bool> InnerExecuteAsync(CancellationToken cancellationToken)
        {
            var gameScreen = ApplicationController.Instance.UIPanelController.GetPanel<UIGameScreen>();
            if (_progressBar)
                gameScreen.SetActiveElement(UIGameScreenElement.ProgressBar, true);
            if (_coins)
                gameScreen.SetActiveElement(UIGameScreenElement.Coins, true);
            if (_buffs)
                gameScreen.SetActiveElement(UIGameScreenElement.Buffs, true);
            if (_settingsBtn)
                gameScreen.SetActiveElement(UIGameScreenElement.Settings, true);
            if (_adsBtn)
                gameScreen.SetActiveElement(UIGameScreenElement.Ads, true);
            return true;
        }
    }
}