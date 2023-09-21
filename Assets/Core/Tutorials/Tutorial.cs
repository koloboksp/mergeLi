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
        
        public TutorialController Controller => _controller;
        public async Task Execute(CancellationToken cancellationToken)
        {
            var steps = GetComponentsInChildren<TutorialStep>().ToList();
            foreach (var tutorialStep in steps)
                await tutorialStep.Execute(cancellationToken);
        }
    }
}