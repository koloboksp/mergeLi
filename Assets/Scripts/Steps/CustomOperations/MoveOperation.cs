using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Steps.CustomOperations
{
    public class MoveOperation : Operation
    {
        private readonly Vector3Int _startPosition;
        private readonly Vector3Int _endPosition;
        private readonly IField _field;
    
        public MoveOperation(Vector3Int startPosition, Vector3Int endPosition, IField field)
        {
            _startPosition = startPosition;
            _endPosition = endPosition;
        
            _field = field;
        }

        protected override async Task<object> InnerExecuteAsync(CancellationToken cancellationToken)
        {
            var movable = _field.GetSomething<IFieldMovable>(_startPosition).ToList();
            var firstMovable = movable[0];
            await firstMovable.StartMove(_endPosition, cancellationToken);

            return null;
        }

        public override Operation GetInverseOperation()
        {
            return new MoveOperation(_endPosition, _startPosition, _field);
        }
    }
}