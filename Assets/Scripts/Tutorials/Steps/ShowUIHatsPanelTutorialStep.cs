using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Tutorials
{
    public class ShowUIHatsPanelTutorialStep : TutorialStep
    {
        [SerializeField] private string[] _hatsFilter;
        protected override async Task<bool> InnerExecuteAsync(CancellationToken cancellationToken)
        {
            var gameProcessor = Tutorial.Controller.GameProcessor;
            
            foreach (var hat in gameProcessor.Scene.HatsLibrary.Hats)
            {
                hat.HackIsFree();
                hat.HackGroupIndex(7);
            }
            
            var data = new UIHatsPanelData();
            data.Layer = "gameScreenFrontLayer";
            data.GameProcessor = gameProcessor;
            data.Selected = gameProcessor.Scene.HatsLibrary.Hats[0];
            data.UserActiveHatsFilter = gameProcessor.Scene.GetUserActiveHatsFilter();
            data.Hats = gameProcessor.Scene.HatsLibrary.Hats
                .Where(hat => _hatsFilter
                    .FirstOrDefault(hatName => hatName == hat.Id) != null)
                .ToList();
            data.HatsChanger = gameProcessor.Scene;
            
            _ = ApplicationController.Instance.UIPanelController.PushPopupScreenAsync<UIHatsPanel>(
                data,
                Application.exitCancellationToken);
            return true;
        }
    }
}