using UnityEngine;

namespace Core.Buffs
{
    public class ExplodeBuff : AreaEffect
    {
        [SerializeField] private ExplodeType _explodeType;

        protected override bool UndoAvailable => true;
        protected override StepTag UndoStepTag => GameProcessor.UndoStepTags[GameProcessor.ExplodeTypeToStepTags[_explodeType]];

        protected override void InnerProcessUsing()
        {
            _gameProcessor.UseExplodeBuff(Cost, _explodeType, AffectedAreas, this);
        }
    }
}