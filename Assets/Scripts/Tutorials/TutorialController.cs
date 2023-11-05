using System;
using System.Collections.Generic;
using System.Linq;
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
        
        private readonly List<Tutorial> _availableTutorials = new List<Tutorial>();
        
        public GameProcessor GameProcessor => _gameProcessor;

        public UITutorialFocuser Focuser => _tutorialFocuser;
        public UITutorialFinger Finger => _tutorialFinger;
        public UITutorialDialog Dialog => _tutorialDialog;

        
        public void Awake()
        {
            gameObject.GetComponentsInChildren(_availableTutorials);
        }

        public async Task TryStartTutorial(bool forceStart, CancellationToken cancellationToken)
        {
            foreach (var tutorial in _availableTutorials)
            {
                if(tutorial.CanStart(forceStart))
                    await tutorial.Execute(cancellationToken);
            }
        }

        public bool CanStartTutorial(bool forceStart)
        {
            return _availableTutorials.Any(tutorial => tutorial.CanStart(forceStart));
        }
    }
}