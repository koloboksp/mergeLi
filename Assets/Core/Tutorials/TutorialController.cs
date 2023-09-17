using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Tutorials
{
    public class TutorialController : MonoBehaviour
    {
        [SerializeField] private UITutorialFocuser _tutorialFocuser;
        [SerializeField] private GameProcessor _gameProcessor;

        [SerializeField] private Tutorial _start;

        public GameProcessor GameProcessor => _gameProcessor;

        public UITutorialFocuser Focuser => _tutorialFocuser;
        
        public async Task TryStartTutorial(CancellationToken cancellationToken)
        {
            await _start.Execute(cancellationToken);
        }
    }
}