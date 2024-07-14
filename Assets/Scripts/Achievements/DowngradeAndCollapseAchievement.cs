using Core;
using Core.Gameplay;
using Core.Steps;
using Core.Steps.CustomOperations;

namespace Achievements
{
    public class DowngradeAndCollapseAchievement : Achievement
    {
        public override void SetData(GameProcessor gameProcessor)
        {
            base.SetData(gameProcessor);

            GameProcessor.OnStepCompleted += GameProcessor_OnStepCompleted;
        }

        private void GameProcessor_OnStepCompleted(Step step, StepExecutionType stepExecutionType)
        {
            if (step.Tag != StepTag.Downgrade)
            {
                return;
            }
        
            var collapseOperationData = step.GetData<CollapseOperationData>();
            if (collapseOperationData != null && collapseOperationData.CollapseLines.Count > 0)
            {
                Unlock();
            }
        }
    }
}