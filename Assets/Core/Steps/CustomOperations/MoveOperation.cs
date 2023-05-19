using System.Linq;
using UnityEngine;

namespace Core.Steps.CustomOperations
{
    public class MoveOperation : Operation
    {
        private Vector3Int _startPosition;
        private Vector3Int _endPosition;
        private readonly IField _field;
    
        public MoveOperation(Vector3Int startPosition, Vector3Int endPosition, IField field)
        {
            _startPosition = startPosition;
            _endPosition = endPosition;
        
            _field = field;
        }

        protected override void InnerExecute()
        {
            var movable = _field.GetSomething<IFieldMovable>(_startPosition).ToList();
            movable[0].StartMove(_endPosition, OnMovingComplete);
            
            Owner.SetData(new MoveOperationData()
            {
                StartPosition = _startPosition,
                EndPosition = _endPosition,
            });
        }

        private void OnMovingComplete(IFieldMovable sender, bool pathFound)
        {
            Complete(null);
        }
    }

    public class MoveOperationData
    {
        public Vector3Int StartPosition;
        public Vector3Int EndPosition;
    }
}