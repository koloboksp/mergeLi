using System.Collections.Generic;
using System.Linq;
using Core.Steps;
using Core.Steps.CustomOperations;
using UnityEngine;

namespace Achievements
{
    public class UseAllBuffsAtLeastNTimesAchievement : Achievement
    {
        [SerializeField] private StepTag[] _buffsTags;
        [SerializeField] private int _threshold = 1;

        private readonly Dictionary<StepTag, int> _usedBuffs = new Dictionary<StepTag, int>();

        public override void SetData(GameProcessor gameProcessor)
        {
            base.SetData(gameProcessor);

            ResetRequiredBuffs();

            GameProcessor.OnStepCompleted += GameProcessor_OnStepCompleted;
        }

        private void ResetRequiredBuffs()
        {
            foreach (var buffsTag in _buffsTags)
                _usedBuffs[buffsTag] = 0;
        }

        private void GameProcessor_OnStepCompleted(Step step, StepExecutionType stepExecutionType)
        {
            if (!_usedBuffs.ContainsKey(step.Tag))
                return;

            _usedBuffs[step.Tag] += 1;

            if (_usedBuffs.All(i => i.Value >= _threshold))
            {
                ResetRequiredBuffs();
                Unlock();
            }
        }
    }
}