using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Gameplay;
using UnityEngine;

namespace Core.Tutorials
{
    public class SetActiveHatsTutorialStep : TutorialStep
    {
        [SerializeField] public string[] _activeHats;
       
        protected override async Task<bool> InnerExecuteAsync(CancellationToken cancellationToken)
        {
            foreach (var activeHat in _activeHats)
            {
                var list = Tutorial.Controller.GameProcessor.Scene.HatsLibrary.Hats
                    .Where(i => i.name == activeHat)
                    .ToList();
                foreach (var hat in list)
                    hat.HackIsFree();
            }
            Tutorial.Controller.GameProcessor.Scene.SetUserActiveHatsFilter(_activeHats);
            
            return true;
        }
    }
}