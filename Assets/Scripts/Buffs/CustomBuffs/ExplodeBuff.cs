using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Effects;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.Buffs
{
    public class ExplodeBuff : AreaBuff
    {
        [SerializeField] private ExplodeType _explodeType;
        [SerializeField] private ExplodeEffect _explodeEffectPrefab;
        [SerializeField] private float _destroyBallsDelay = 0.5f;

        protected virtual bool UndoAvailable => true;
        protected virtual StepTag UndoStepTag => GameProcessor.UndoStepTags[GameProcessor.ExplodeTypeToStepTags[_explodeType]];

        protected override void InnerProcessUsing(PointerEventData eventData)
        {
            _ = ExplodeWithEffectAsync(eventData, Application.exitCancellationToken);
        }

        protected virtual async Task ExplodeWithEffectAsync(PointerEventData eventData, CancellationToken cancellationToken)
        {
            var screenPointToWorld = _gameProcessor.Scene.Field.ScreenPointToWorld(eventData.position);
            var pointerGridPosition = _gameProcessor.Scene.Field.GetPointGridIntPosition(screenPointToWorld);

            screenPointToWorld.z = 0;
            var explodeEffect = Instantiate(_explodeEffectPrefab, 
                screenPointToWorld, Quaternion.identity, 
                _gameProcessor.UIFxLayer.transform);

            explodeEffect.Run(0);
            await AsyncExtensions.WaitForSecondsAsync(_destroyBallsDelay, cancellationToken);
            _gameProcessor.UseExplodeBuff(Cost, _explodeType, pointerGridPosition, AffectedAreas, this);
        }

        public override string Id => _explodeType.ToString();
    }
}