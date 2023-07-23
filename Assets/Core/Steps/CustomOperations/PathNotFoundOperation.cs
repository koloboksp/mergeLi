using System.Linq;
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

        protected override void InnerExecute()
        {
            var balls = _field.GetSomething<Ball>(_startPosition).ToList();
            if (balls.Count > 0)
                balls[0].PathNotFound();
            
            var position = _field.View.Root.TransformPoint(_field.GetPositionFromGrid(_endPosition));
            var noPathEffect = Object.Instantiate(_noPathEffect, position, Quaternion.identity, _field.View.Root);
            noPathEffect.Run();
            
            Complete(null);
        }
    }
}