using System.Collections.Generic;
using System.Threading.Tasks;
using Atom;
using Core;
using Core.Gameplay;

namespace Analytics
{
    public class DefaultAnalytics : IAnalyticsController
    {
        public Task InitializeAsync(Version version)
        {
            return Task.CompletedTask;
        }

        public void TutorialStepCompleted(string stepName)
        {
           
        }

        public void StepIntervalCompleted(int step, string castleId, IReadOnlyList<StepTakeIntoInfo> stepsTakenIntoInfo)
        {
           
        }

        public void BuffUsed(StepTag buffTag, string castleId, int step, int restFieldEmptyCellsCount)
        {
            
        }

        public void AdsViewed(string adsName, int rewardAmount, string castleId, int step)
        {
          
        }

        public void OnLost(string castleId, int step, IReadOnlyList<StepTakeIntoInfo> stepsTakenIntoInfo)
        {
            
        }

        public void OnRestart(string castleId, int step, int restFieldEmptyCellsCount, IReadOnlyList<StepTakeIntoInfo> stepsTakenIntoInfo)
        {
           
        }

        public void OnGiftCollected(string id, int rewardAmount, string castleId, int step)
        {
            
        }

        public void OnMarketBought(string id, int rewardAmount, string castleId, int step, int restFieldEmptyCellsCount)
        {
       
        }

        public void OnCastleComplete(string castleId, int step)
        {
            
        }
    }
}