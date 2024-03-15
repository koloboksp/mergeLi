using System.Linq;
using Core.Effects;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.Buffs
{
    public class ExplodeBuff : AreaBuff
    {
        [SerializeField] private ExplodeType _explodeType;
        [SerializeField] private ExplodeEffect _explodeEffectPrefab;

        protected virtual bool UndoAvailable => true;
        protected virtual StepTag UndoStepTag => GameProcessor.UndoStepTags[GameProcessor.ExplodeTypeToStepTags[_explodeType]];

        protected override void InnerProcessUsing(PointerEventData eventData)
        {
            var pointerGridPosition = _gameProcessor.Scene.Field.GetPointGridIntPosition(_gameProcessor.Scene.Field.ScreenPointToWorld(eventData.position));
            _gameProcessor.UseExplodeBuff(Cost, _explodeType, pointerGridPosition, AffectedAreas, this);

            var maxDistanceToCheckingPosition = AffectedAreas.Max(i => (i - pointerGridPosition).magnitude);
            foreach (var areaGridPosition in AffectedAreas)
            {
                var explodeEffect = Instantiate(_explodeEffectPrefab, 
                    _gameProcessor.Scene.Field.View.Root.TransformPoint(_gameProcessor.Scene.Field.GetPositionFromGrid(areaGridPosition)), Quaternion.identity, 
                    _gameProcessor.UIFxLayer.transform);
                
                explodeEffect.Run((areaGridPosition - pointerGridPosition).magnitude / maxDistanceToCheckingPosition);
            }
        }

        public override string Id => _explodeType.ToString();
    }
}