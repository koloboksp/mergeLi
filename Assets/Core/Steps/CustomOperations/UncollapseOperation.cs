using System.Collections.Generic;
using UnityEngine;

namespace Core.Steps.CustomOperations
{
    public class UncollapseOperation : Operation
    {
        private readonly IField _field;
        private readonly IPointsChangeListener _pointsChangeListener;

        private readonly List<List<(Vector3Int intPosition, int points)>> _uncollapseBallsLines;
        private readonly int _pointsToRemove;
        
        public UncollapseOperation(List<List<(Vector3Int intPosition, int points)>> uncollapseBallsLines, int pointsToRemove,
            IField field, IPointsChangeListener pointsChangeListener)
        {
            _uncollapseBallsLines = uncollapseBallsLines;
            _pointsToRemove = pointsToRemove;
            _field = field;
            _pointsChangeListener = pointsChangeListener;
        }

        protected override void InnerExecute()
        {
            var uniqueBalls = new List<(Vector3Int intPosition, int points)>();
            foreach (var uncollapseBallsLine in _uncollapseBallsLines)
            foreach (var uncollapseBall in uncollapseBallsLine)
                {
                    var foundBallI = uniqueBalls.FindIndex(i => i.intPosition == uncollapseBall.intPosition);
                    if (foundBallI < 0)
                        uniqueBalls.Add(uncollapseBall);
                }
            
            foreach (var uniqueBall in uniqueBalls)
                _field.CreateBall(uniqueBall.intPosition, uniqueBall.points);

            _pointsChangeListener.RemovePoints(_pointsToRemove);
            
            Complete(null);
        }
    }
}