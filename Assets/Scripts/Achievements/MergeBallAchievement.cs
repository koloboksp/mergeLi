using System.Linq;
using Core;
using Core.Gameplay;
using Core.Steps;
using Core.Steps.CustomOperations;
using UnityEngine;

namespace Achievements
{
    public class MergeBallAchievement : Achievement
    {
        [SerializeField] private int _targetBallPoints;

        public override void SetData(GameProcessor gameProcessor)
        {
            base.SetData(gameProcessor);

            GameProcessor.OnStepCompleted += GameProcessor_OnStepCompleted;
        }
        
        private void GameProcessor_OnStepCompleted(Step step, StepExecutionType stepExecutionType)
        {
            if (step.Tag != StepTag.Merge)
            {
                return;
            }

            if (stepExecutionType != StepExecutionType.Redo)
            {
                return;
            }

            var mergeData = step.GetData<MergeOperationData>();
            if (mergeData == null)
            {
                return;
            }

            if (mergeData.MergedBalls.Sum(i => i.Points) == _targetBallPoints)
            {
                Unlock();
            }
        }
    }
}