using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Effects;
using Core.Gameplay;
using UnityEngine;

namespace Core.Steps.CustomOperations
{
    public class MoveOperation : Operation
    {
        private readonly Vector3Int _startPosition;
        private readonly Vector3Int _endPosition;
        private readonly IField _field;
        private readonly MoveBallEndEffect _moveBallEndEffectPrefab;
        
        public MoveOperation(
            Vector3Int startPosition,
            Vector3Int endPosition,
            IField field,
            MoveBallEndEffect moveBallEndEffect)
        {
            _startPosition = startPosition;
            _endPosition = endPosition;
        
            _field = field;
            _moveBallEndEffectPrefab = moveBallEndEffect;
        }

        protected override async Task<object> InnerExecuteAsync(CancellationToken cancellationToken)
        {
            var movable = _field.GetSomething<IFieldMovable>(_startPosition).ToList();
            var firstMovable = movable[0];
            await firstMovable.StartMove(_endPosition, cancellationToken);
           
            var movablePosition = _field.GetPositionFromGrid(firstMovable.IntGridPosition);
            var findObjectOfType = GameObject.FindObjectOfType<UIFxLayer>();
           
            if (_moveBallEndEffectPrefab != null)
            {
                var moveBallEndEffect = Object.Instantiate(
                    _moveBallEndEffectPrefab, 
                    _field.View.Root.TransformPoint(movablePosition), 
                    Quaternion.identity, 
                    findObjectOfType.transform);
                moveBallEndEffect.Run();
            }
            
            return null;
        }

        public override Operation GetInverseOperation()
        {
            return new MoveOperation(_endPosition, _startPosition, _field, null);
        }
    }
}