using System.Linq;
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
    
        protected override void InnerExecute()
        {
            var foundBalls = _field.GetSomething<Ball>(_position).ToList();
            _field.DestroyBalls(foundBalls);
            for (int ballI = 0; ballI < _mergeablesNum; ballI++)
                _field.CreateBall(_position, _points);
            
            Complete(null);
        }
    }
}