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
        [SerializeField] private string _id;
        [SerializeField] private TutorialController _controller;
        [SerializeField] private TutorialEntry _entry;
        [SerializeField] private TutorialStep _startStep;
        [SerializeField] private List<TutorialStep> _requiredSteps = new List<TutorialStep>();

        public string Id => _id;
        
        public TutorialController Controller => _controller;
       
        public async Task Execute(CancellationToken cancellationToken)
        {
            var steps = new List<TutorialStep>();
            FindAllScriptsWithoutNested<TutorialStep>(transform, steps);
            var findIndex = steps.FindIndex(i => i == _startStep);
            var startTutorialIndex = findIndex >= 0 ? findIndex : 0;

            if (_startStep != null)
            {
                foreach (var requiredStep in _requiredSteps)
                    await requiredStep.Execute(cancellationToken);
            }
            
            for (var stepI = startTutorialIndex; stepI < steps.Count; stepI++)
            {
                var tutorialStep = steps[stepI];
                await tutorialStep.Execute(cancellationToken);
            }
        }
        
        private void FindAllScriptsWithoutNested<T>(Transform target, List<T> result) where T: MonoBehaviour
        {
            for (int i = 0; i < target.childCount; i++)
            {
                var child = target.GetChild(i);
                var noAllocFoundMonoBehaviours = new List<T>();
                child.gameObject.GetComponents(noAllocFoundMonoBehaviours);
                if (noAllocFoundMonoBehaviours.Count > 0)
                {
                    foreach (var monoBehaviour in noAllocFoundMonoBehaviours)
                        result.Add(monoBehaviour);
                }
                else
                    FindAllScriptsWithoutNested(child, result);
            }
        }

        public bool CanStart(bool forceStart) => _entry.CanStart(forceStart);

        public void Complete()
        {
            ApplicationController.Instance.SaveController.CompleteTutorial(Id);
        }
    }
}