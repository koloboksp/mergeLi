using System.Collections.Generic;
using System.Threading.Tasks;
using Atom;

namespace Core
{
    public interface IAnalyticsController
    {
        Task InitializeAsync(Version version);

        void TutorialStepCompleted(string stepName);
        void StepIntervalCompleted(int step, string castleId, IReadOnlyList<(StepTag stepTag, int count)> stepsTakenIntoInfo);
        void BuffUsed(StepTag buffTag, string castleId, int step, int restFieldEmptyCellsCount);
        void AdsViewed(string adsName, int rewardAmount, string castleId, int step);
        void OnLost(string castleId, int step, IReadOnlyList<(StepTag stepTag, int count)> stepsTakenIntoInfo);
        void OnRestart(string castleId, int step, int restFieldEmptyCellsCount, IReadOnlyList<(StepTag stepTag, int count)> stepsTakenIntoInfo);
        void OnGiftCollected(string id, int rewardAmount, string castleId, int step);
        void OnMarketBought(string id, int rewardAmount, string castleId, int step, int restFieldEmptyCellsCount);
        void OnCastleComplete(string castleId, int step);
    }
}