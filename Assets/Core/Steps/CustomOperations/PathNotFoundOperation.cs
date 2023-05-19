using UnityEngine;

namespace Core.Steps.CustomOperations
{
    public class PathNotFoundOperation : Operation
    {
        private Vector3Int _startPosition;
        private Vector3Int _endPosition;
        private readonly IField _field;
        public PathNotFoundOperation(Vector3Int startPosition, Vector3Int endPosition, IField field)
        {
            _startPosition = startPosition;
            _endPosition = endPosition;
            _field = field;
        }

        protected override void InnerExecute()
        {
            Complete(null);
        }
    }
}