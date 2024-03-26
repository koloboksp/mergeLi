using System.Collections.Generic;
using Core.Steps;
using Core.Steps.CustomOperations;
using UnityEngine;

namespace Achievements
{
    public class NonCollapseBallsNTimesInRowAchievement : Achievement
    {
        [SerializeField] private int _threshold = 10;

        private int _times = 0;
    
        public override void SetData(GameProcessor gameProcessor)
        {
            base.SetData(gameProcessor);

            GameProcessor.OnStepCompleted += GameProcessor_OnStepCompleted;
        }

        private void GameProcessor_OnStepCompleted(Step step, StepExecutionType stepExecutionType)
        {
            if (step.Tag != StepTag.Move && step.Tag != StepTag.Merge && step.Tag != StepTag.Downgrade)
            {
                return;
            }

            if (stepExecutionType != StepExecutionType.Redo)
            {
                _times = 0;
                return;
            }

            var collapseOperationData = step.GetData<CollapseOperationData>();
            if (collapseOperationData != null && collapseOperationData.CollapseLines.Count > 0)
            {
                _times = 0;
                return;
            }
        
            _times++;

            if (_times >= _threshold)
            {
                _times = 0;
                Unlock();
            }
        }
    }
}