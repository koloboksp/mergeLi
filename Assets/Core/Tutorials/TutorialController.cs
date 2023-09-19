using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Tutorials
{
    public class TutorialController : MonoBehaviour
    {
        [SerializeField] private UITutorialFocuser _tutorialFocuser;
        [SerializeField] private UITutorialFinger _tutorialFinger;
        [SerializeField] private UITutorialDialog _tutorialDialog;

        [SerializeField] private GameProcessor _gameProcessor;

        [SerializeField] private Tutorial _start;

        public GameProcessor GameProcessor => _gameProcessor;

        public UITutorialFocuser Focuser => _tutorialFocuser;
        public UITutorialFinger Finger => _tutorialFinger;
        public UITutorialDialog Dialog => _tutorialDialog;

        public async Task TryStartTutorial(CancellationToken cancellationToken)
        {
            await _start.Execute(cancellationToken);
        }
    }
}