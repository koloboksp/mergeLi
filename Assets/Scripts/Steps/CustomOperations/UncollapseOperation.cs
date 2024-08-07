using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Effects;
using Core.Gameplay;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.Steps.CustomOperations
{
    public class UncollapseOperation : Operation
    {
        private readonly IField _field;
        private readonly ISessionStatisticsHolder _statisticsHolder;
        private readonly GameProcessor _gameProcessor;
        
        private readonly List<List<BallDesc>> _uncollapseBallsLines;
        private readonly int _pointsToRemove;
        private readonly int _coinsToRemove;

        public UncollapseOperation(
            List<List<BallDesc>> uncollapseBallsLines,
            int pointsToRemove,
            int coinsToRemove,
            IField field,
            ISessionStatisticsHolder statisticsHolder,
            GameProcessor gameProcessor)
        {
            _uncollapseBallsLines = uncollapseBallsLines;
            _pointsToRemove = pointsToRemove;
            _field = field;
            _statisticsHolder = statisticsHolder;
            _gameProcessor = gameProcessor;
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
            _statisticsHolder.ChangeCollapseLinesCount(-_uncollapseBallsLines.Count);
            
            foreach (var uniqueBall in uniqueBalls)
                _field.CreateBall(uniqueBall.GridPosition, uniqueBall.Points, uniqueBall.HatName);

            var receivers = SceneManager.GetActiveScene().GetRootGameObjects()
                .SelectMany(i => i.GetComponentsInChildren<IPointsEffectReceiver>())
                .OrderBy(i => i.Priority)
                .ToList();
            foreach (var receiver in receivers)
                receiver.Refund(_pointsToRemove);
            
            _gameProcessor.ConsumeCoins(_coinsToRemove);
            
            return null;
        }
    }
}