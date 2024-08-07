﻿using System.Threading;
using System.Threading.Tasks;

namespace Core.Tutorials
{
    public class ShowUIGameScreenTutorialStep : TutorialStep
    {
        protected override async Task<bool> InnerExecuteAsync(CancellationToken cancellationToken)
        {
            var gameScreen = await ApplicationController.Instance.UIPanelController.PushPopupScreenAsync<UIGameScreen>(
                new UIGameScreenData()
                {
                    Layer = "gameScreenLayer",
                    GameProcessor = Tutorial.Controller.GameProcessor
                }, 
                cancellationToken);
            gameScreen.HideAllElements();
            ApplicationController.UnloadLogoScene();
            
            return true;
        }
    }
}