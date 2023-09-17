using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Tutorials
{
    public class Tutorial : MonoBehaviour
    {
        [SerializeField] private TutorialController _controller;
        [SerializeField] private List<TutorialStep> _steps;

        public TutorialController Controller => _controller;
        public async Task Execute(CancellationToken cancellationToken)
        {
            foreach (var tutorialStep in _steps)
                await tutorialStep.Execute(cancellationToken);
        }
    }
}