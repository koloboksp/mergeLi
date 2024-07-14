using Core;
using Core.Gameplay;
using Core.Steps;
using UnityEngine;

namespace Achievements
{
    public class UseBuffAtLeastNTimesInRowAchievement : Achievement
    {
        [SerializeField] private StepTag _buffTag;
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
            if (stepExecutionType == StepExecutionType.Undo)
                return;

            if (step.Tag == StepTag.Select 
                || step.Tag == StepTag.Deselect 
                || step.Tag == StepTag.ChangeSelected
                || step.Tag == StepTag.NoPath)
                return;

            if (_buffTag != step.Tag)
            {
                _times = 0;
                return;
            }

            _times += 1;

            if (_times >= _threshold)
            {
                _times = 0;
                Unlock();
            }
        }
    }
}