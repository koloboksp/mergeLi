using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Tutorials
{
    public class GiveCoinsTutorialStep : TutorialStep
    {
        [SerializeField] private int _coinsAmount = 50;
        
        protected override async Task<bool> InnerExecute(CancellationToken cancellationToken)
        {
            await Tutorial.Controller.GameProcessor.GiveTutorialCoins(_coinsAmount);
           
            return true;
        }
    }
}