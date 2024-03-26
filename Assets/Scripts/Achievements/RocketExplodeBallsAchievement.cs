using System.Linq;
using Core.Steps;
using Core.Steps.CustomOperations;
using UnityEngine;

namespace Achievements
{
    public class RocketExplodeBallsAchievement : Achievement
    {
        [SerializeField] private int[] _ballValues;
        
        public override void SetData(GameProcessor gameProcessor)
        {
            base.SetData(gameProcessor);

            GameProcessor.OnStepCompleted += GameProcessor_OnStepCompleted;
        }

        private void GameProcessor_OnStepCompleted(Step step, StepExecutionType stepExecutionType)
        {
            if (step.Tag != StepTag.ExplodeHorizontal && step.Tag != StepTag.ExplodeVertical)
            {
                return;
            }

            if (stepExecutionType != StepExecutionType.Redo)
            {
                return;
            }

            var foundOperation = step.FindOperation<RemoveOperation>();
            if (foundOperation == null || foundOperation.RemovedBalls.Count == 0)
            {
                return;
            }

            var groupByPoints = foundOperation.RemovedBalls
                .GroupBy(i => i.Points)
                .ToList();

            var allBallsGroupsFound = true;
            foreach (var ballValue in _ballValues)
            {
                var ballGroup = groupByPoints.FirstOrDefault(i => i.Key == ballValue);
                if (ballGroup == null)
                    allBallsGroupsFound = false;
            }
            
            if (allBallsGroupsFound)
            {
                Unlock();
            }
        }
    }
}