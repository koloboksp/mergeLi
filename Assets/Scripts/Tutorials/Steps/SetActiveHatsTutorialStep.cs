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
        [SerializeField] public Scene _scene;

        protected override async Task<bool> InnerExecuteAsync(CancellationToken cancellationToken)
        {
            foreach (var activeHat in _activeHats)
            {
                var list = _scene.HatsLibrary.Hats
                    .Where(i => i.name == activeHat)
                    .ToList();
                foreach (var hat in list)
                    hat.HackIsFree();
            }
            _scene.SetUserActiveHatsFilter(_activeHats);
            
            return true;
        }
    }
}