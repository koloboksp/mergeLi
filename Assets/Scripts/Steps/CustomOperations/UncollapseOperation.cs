using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Steps.CustomOperations
{
    public class UncollapseOperation : Operation
    {
        private readonly IField _field;
        private readonly IPointsChangeListener _pointsChangeListener;

        private readonly List<List<BallDesc>> _uncollapseBallsLines;
        private readonly int _pointsToRemove;
        
        public UncollapseOperation(List<List<BallDesc>> uncollapseBallsLines, int pointsToRemove,
            IField field, IPointsChangeListener pointsChangeListener)
        {
            _uncollapseBallsLines = uncollapseBallsLines;
            _pointsToRemove = pointsToRemove;
            _field = field;
            _pointsChangeListener = pointsChangeListener;
        }

        protected override async Task<object> InnerExecuteAsync(CancellationToken cancellationToken)
        {
            var uniqueBalls = new List<BallDesc>();
            foreach (var uncollapseBallsLine in _uncollapseBallsLines)
            foreach (var uncollapseBall in uncollapseBallsLine)
            {
                var foundBallI = uniqueBalls.FindIndex(i => i.GridPosition == uncollapseBall.GridPosition);
                if (foundBallI < 0)
                    uniqueBalls.Add(uncollapseBall);
            }
            
            foreach (var uniqueBall in uniqueBalls)
                _field.CreateBall(uniqueBall.GridPosition, uniqueBall.Points);

            _pointsChangeListener.RemovePoints(_pointsToRemove);

            return null;
        }
    }
}