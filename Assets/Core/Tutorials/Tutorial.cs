using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Tutorials
{
    public class Tutorial : MonoBehaviour
    {
        [SerializeField] private TutorialController _controller;
        [SerializeField] private TutorialStep _startStep;
        [SerializeField] private List<TutorialStep> _requiredSteps = new List<TutorialStep>();
        public TutorialController Controller => _controller;
        public async Task Execute(CancellationToken cancellationToken)
        {
            var steps = GetComponentsInChildren<TutorialStep>().ToList();
            var findIndex = steps.FindIndex(i => i == _startStep);
            var startTutorialIndex = findIndex >= 0 ? findIndex : 0;

            if (_startStep != null)
            {
                foreach (var requiredStep in _requiredSteps)
                {
                    await requiredStep.Execute(cancellationToken);
                }
            }
            
            for (var stepI = startTutorialIndex; stepI < steps.Count; stepI++)
            {
                var tutorialStep = steps[stepI];
                if (_startStep != null)
                {
                    await tutorialStep.Execute(cancellationToken);
                }
            }
        }
    }
}