using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Effects;
using UnityEngine;

namespace Core.Steps.CustomOperations
{
    public class RemoveOperation : Operation
    {
        private readonly IField _field;
        private readonly Vector3Int _pointerIndex;
        private readonly List<Vector3Int> _indexes;
        private readonly DestroyBallEffect _destroyBallEffectPrefab;
        
        private readonly List<(Vector3Int, int)> _removedBalls = new();
        
        public RemoveOperation(Vector3Int pointerIndex, IEnumerable<Vector3Int> indexes, DestroyBallEffect destroyBallEffectPrefab, IField field)
        {
            _pointerIndex = pointerIndex;
            _indexes = new List<Vector3Int>(indexes);
            _destroyBallEffectPrefab = destroyBallEffectPrefab;
            _field = field;
        }
        
        protected override void InnerExecute()
        {
            var foundBalls = _indexes
                .SelectMany(i => _field.GetSomething<Ball>(i))
                .ToList();

            if (foundBalls.Count != 0)
            {
                var maxDistanceToCheckingPosition = foundBalls.Max(ball => (ball.IntGridPosition - _pointerIndex).magnitude);
                foreach (var ball in foundBalls)
                {
                    var destroyBallEffect = Object.Instantiate(_destroyBallEffectPrefab,
                        _field.View.Root.TransformPoint(_field.GetPositionFromGrid(ball.IntGridPosition)),
                        Quaternion.identity,
                        _field.View.Root);

                    var distanceToBall = (ball.IntGridPosition - _pointerIndex).magnitude;
                    destroyBallEffect.Run(ball.GetColorIndex(), distanceToBall / maxDistanceToCheckingPosition);
                }

                _removedBalls.AddRange(foundBalls.Select(i => (IntPosition: i.IntGridPosition, i.Points)));
                _field.DestroyBalls(foundBalls);
            }

            Complete(null);
        }

        public override Operation GetInverseOperation()
        {
            return new AddOperation(_removedBalls, _field);
        }
    }
}