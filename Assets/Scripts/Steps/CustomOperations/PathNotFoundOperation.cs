using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Gameplay;
using UnityEngine;

namespace Core.Steps.CustomOperations
{
    public class PathNotFoundOperation : Operation
    {
        private Vector3Int _startPosition;
        private Vector3Int _endPosition;
        private readonly NoPathEffect _noPathEffect;
        
        private readonly IField _field;
        
        public PathNotFoundOperation(Vector3Int startPosition, Vector3Int endPosition, NoPathEffect noPathEffect, IField field)
        {
            _startPosition = startPosition;
            _endPosition = endPosition;
            _noPathEffect = noPathEffect;
            _field = field;
        }

        protected override async Task<object> InnerExecuteAsync(CancellationToken cancellationToken)
        {
            var balls = _field.GetSomething<Ball>(_startPosition).ToList();
            if (balls.Count > 0)
                balls[0].PathNotFound();
            
            var position = _field.View.Root.TransformPoint(_field.GetPositionFromGrid(_endPosition));
            var noPathEffect = Object.Instantiate(_noPathEffect, position, Quaternion.identity, _field.View.Root);
            noPathEffect.AdjustSize(_field.CellSize());
            noPathEffect.Run();

            return null;
        }
    }
}