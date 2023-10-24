using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.Buffs
{
    public class ExplodeBuff : AreaEffect
    {
        [SerializeField] private ExplodeType _explodeType;

        protected virtual bool UndoAvailable => true;
        protected virtual StepTag UndoStepTag => GameProcessor.UndoStepTags[GameProcessor.ExplodeTypeToStepTags[_explodeType]];

        protected override void InnerProcessUsing(PointerEventData eventData)
        {
            var pointerGridPosition = _gameProcessor.Scene.Field.GetPointGridIntPosition(_gameProcessor.Scene.Field.ScreenPointToWorld(eventData.position));
            _gameProcessor.UseExplodeBuff(Cost, _explodeType, pointerGridPosition, AffectedAreas, this);
        }

        public override string Id => _explodeType.ToString();
    }
}