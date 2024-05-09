using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Steps.CustomOperations
{
    public class UnmergeOperation : Operation
    {
        private readonly List<BallDesc> _sourceBalls;
        private readonly IField _field;

        public UnmergeOperation(List<BallDesc> sourceBalls, IField field)
        {
            _sourceBalls = new List<BallDesc>(sourceBalls);
            _field = field;
        }
    
        protected override async Task<object> InnerExecuteAsync(CancellationToken cancellationToken)
        {
            if (_sourceBalls.Count > 0)
            {
                var foundBalls = _field.GetSomething<Ball>(_sourceBalls[0].GridPosition)
                    .ToList();
                _field.DestroyBalls(foundBalls, true);

                for (var ballI = 0; ballI < _sourceBalls.Count; ballI++)
                {
                    var sourceBall = _sourceBalls[ballI];
                    _field.CreateBall(sourceBall.GridPosition, sourceBall.Points, sourceBall.HatName);
                }
            }
            

            return null;
        }
    }
}