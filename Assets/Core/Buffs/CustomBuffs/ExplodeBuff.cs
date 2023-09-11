using UnityEngine;

namespace Core.Buffs
{
    public class ExplodeBuff : AreaEffect
    {
        [SerializeField] private ExplodeType _explodeType;

        protected virtual bool UndoAvailable => true;
        protected virtual StepTag UndoStepTag => GameProcessor.UndoStepTags[GameProcessor.ExplodeTypeToStepTags[_explodeType]];

        protected override void InnerProcessUsing()
        {
            _gameProcessor.UseExplodeBuff(Cost, _explodeType, AffectedAreas, this);
        }

        public override string Id => _explodeType.ToString();
    }
}