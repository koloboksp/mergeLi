using System.Collections.Generic;
using UnityEngine;

namespace Core.Steps.CustomOperations
{
    public class UncollapseOperation : Operation
    {
        private readonly IField _field;
        private List<List<(Vector3Int position, int points)>> _uncollapseBallsLines;
        
        public UncollapseOperation(List<List<(Vector3Int position, int points)>> uncollapseBallsLines, IField field)
        {
            _uncollapseBallsLines = uncollapseBallsLines;
            _field = field;
        }

        protected override void InnerExecute()
        {
            var uniqueBalls = new List<(Vector3Int position, int points)>();
            foreach (var uncollapseBallsLine in _uncollapseBallsLines)
            foreach (var uncollapseBall in uncollapseBallsLine)
                {
                    var foundBallI = uniqueBalls.FindIndex(i => i.position == uncollapseBall.position);
                    if (foundBallI < 0)
                        uniqueBalls.Add(uncollapseBall);
                }
            
            foreach (var uniqueBall in uniqueBalls)
                _field.CreateBall(uniqueBall.position, uniqueBall.points);

            Complete(null);
        }
    }
}