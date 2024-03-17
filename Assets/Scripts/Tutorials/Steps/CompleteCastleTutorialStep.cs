using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Atom;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.Tutorials
{
    public class CompleteCastleTutorialStep : TutorialStep
    {
        [SerializeField] private List<TutorialStep> _stepsBeforeGetCoins = new List<TutorialStep>();
        [SerializeField] private List<TutorialStep> _stepsBeforeSelectNextCastle = new List<TutorialStep>();
        [SerializeField] private List<TutorialStep> _stepsAfterSelectNextCastle = new List<TutorialStep>();

        protected override async Task<bool> InnerExecuteAsync(CancellationToken cancellationToken)
        {
            Tutorial.Controller.GameProcessor.CastleSelector.ForceCompleteCastle();
            await Tutorial.Controller.GameProcessor.ProcessCastleCompleteAsync(
                GuidEx.Empty,
                StepsBeforeGetCoins,
                BeforeSelectNextCastle,
                AfterSelectNextCastle);

            return true;

            async Task StepsBeforeGetCoins()
            {
                foreach (var step in _stepsBeforeGetCoins)
                    await step.Execute(cancellationToken);
            }
            
            async Task BeforeSelectNextCastle()
            {
                foreach (var step in _stepsBeforeSelectNextCastle)
                    await step.Execute(cancellationToken);
            }
            
            async Task AfterSelectNextCastle()
            {
                foreach (var step in _stepsAfterSelectNextCastle)
                    await step.Execute(cancellationToken);
            }
        }
    }
}