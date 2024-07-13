using Core;
using Core.Steps;
using Core.Steps.CustomOperations;

namespace Achievements
{
    public class SurviveAchievement : Achievement
    {
        private bool _ballsCollapsedInThisStep = false;
        private bool _freeSpaceIsOverInThisStep = false;

        public override void SetData(GameProcessor gameProcessor)
        {
            base.SetData(gameProcessor);
            
            GameProcessor.OnStepCompleted += GameProcessor_OnStepCompleted;
            GameProcessor.SessionProcessor.OnFreeSpaceIsOverChanged += GameProcessor_OnFreeSpaceIsOverChanged;
        }

        private void GameProcessor_OnStepCompleted(Step step, StepExecutionType stepExecutionType)
        {
            if (step.Tag != StepTag.Move && step.Tag != StepTag.Merge && step.Tag != StepTag.Downgrade)
            {
                _ballsCollapsedInThisStep = false;
                return;
            }

            if (stepExecutionType != StepExecutionType.Redo)
            {
                _ballsCollapsedInThisStep = false;
                return;
            }
            
            var collapseOperationData = step.GetData<CollapseOperationData>();
            if (collapseOperationData != null && collapseOperationData.CollapseLines.Count > 0)
            {
                _ballsCollapsedInThisStep = true;
            }
            
            Check();
        }
        private void GameProcessor_OnFreeSpaceIsOverChanged(bool state, bool noAvailableSteps)
        {
            _freeSpaceIsOverInThisStep = state;
            Check();
        }

        private void Check()
        {
            if (_ballsCollapsedInThisStep && _freeSpaceIsOverInThisStep)
            {
                _ballsCollapsedInThisStep = false;
                _freeSpaceIsOverInThisStep = false;
                
                Unlock();
            }
        }
    }
}