using Core;
using Core.Steps;
using UnityEngine;

namespace Achievements
{
    public class SurviveNTurnsAchievement : Achievement
    {
        [SerializeField] private int _threshold = 1;

        private int _times = 0;

        public override void SetData(GameProcessor gameProcessor)
        {
            base.SetData(gameProcessor);

            _times = 0;

            GameProcessor.OnStepCompleted += GameProcessor_OnStepCompleted;
        }

        private void GameProcessor_OnStepCompleted(Step step, StepExecutionType stepExecutionType)
        {
            if (step.Tag != StepTag.Move && step.Tag != StepTag.Merge)
            {
                return;
            }

            if (stepExecutionType == StepExecutionType.Undo)
            {
                _times -= 1;
            }
            else
            {
                _times += 1;
            }
            
            if (_times >= _threshold)
            {
                _times = 0;
                Unlock();
            }
        }
    }
}