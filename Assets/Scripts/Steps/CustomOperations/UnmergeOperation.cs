using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Steps.CustomOperations
{
    public class UnmergeOperation : Operation
    {
        private readonly Vector3Int _position;
        private readonly int _points;
        private readonly int _mergeablesNum;

        private readonly IField _field;

        public UnmergeOperation(Vector3Int position, int points, int mergeablesNum, IField field)
        {
            _position = position;
            _points = points;
            _mergeablesNum = mergeablesNum;
            _field = field;
        }
    
        protected override async Task<object> InnerExecuteAsync(CancellationToken cancellationToken)
        {
            var foundBalls = _field.GetSomething<Ball>(_position).ToList();
            _field.DestroyBalls(foundBalls, true);
            for (int ballI = 0; ballI < _mergeablesNum; ballI++)
                _field.CreateBall(_position, _points);

            return null;
        }
    }
}