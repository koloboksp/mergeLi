using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Tutorials
{
    public class SetActiveHatsTutorialStep : TutorialStep
    {
        [SerializeField] public string[] _activeHats;
        [SerializeField] public Scene _scene;

        protected override async Task<bool> InnerExecuteAsync(CancellationToken cancellationToken)
        {
            _scene.SetUserActiveHatsFilter(_activeHats);
            
            return true;
        }
    }
}