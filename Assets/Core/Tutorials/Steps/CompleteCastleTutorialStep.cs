using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Tutorials
{
    public class CompleteCastleTutorialStep : TutorialStep
    {
        [SerializeField] private List<TutorialStep> _steps = new List<TutorialStep>();
        
        protected override async Task<bool> InnerExecute(CancellationToken cancellationToken)
        {
            Tutorial.Controller.GameProcessor.CastleSelector.ForceCompleteCastle();
            Tutorial.Controller.GameProcessor.ProcessCastleComplete(Sss);

            return true;

            async Task Sss()
            {
                foreach (var VARIABLE in _steps)
                {
                    await VARIABLE.Execute(cancellationToken);
                }
            }
        }

        
    }
}