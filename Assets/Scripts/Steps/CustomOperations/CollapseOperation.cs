using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Effects;
using Core.Gameplay;
using Core.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.Steps.CustomOperations
{
    public class CollapseOperation : Operation
    {
        private readonly PositionSource _positionSource;
        private readonly Vector3Int _position;
        private readonly bool _selectState;
        private readonly IField _field;
        private readonly IPointsCalculator _pointsCalculator;
        private readonly CollapsePointsEffect _collapsePointsEffectPrefab;
        private readonly ISessionStatisticsHolder _statisticsHolder;
        
        private readonly List<(Ball ball, float distance)> _ballsToRemove = new();
        private readonly List<List<BallDesc>> _collapseLines = new();
        private int _pointsAdded;

        public CollapseOperation(
            Vector3Int position, 
            CollapsePointsEffect collapsePointsEffectPrefab,
            IField field, 
            IPointsCalculator pointsCalculator,
            ISessionStatisticsHolder statisticsHolder)
        {
            _positionSource = PositionSource.Fixed;
            _position = position;
            _field = field;
            _pointsCalculator = pointsCalculator;
            _collapsePointsEffectPrefab = collapsePointsEffectPrefab;
            _statisticsHolder = statisticsHolder;
        }

        public CollapseOperation(
            CollapsePointsEffect collapsePointsEffectPrefab,
            IField field, 
            IPointsCalculator pointsCalculator,
            ISessionStatisticsHolder statisticsHolder)
        {
            _positionSource = PositionSource.FromData;
            _field = field;
            _pointsCalculator = pointsCalculator;
            _collapsePointsEffectPrefab = collapsePointsEffectPrefab;
            _statisticsHolder = statisticsHolder;
        }
    
        protected override async Task<object> InnerExecuteAsync(CancellationToken cancellationToken)
        {
            var checkingPositions = new List<Vector3Int>();
        
            if (_positionSource == PositionSource.Fixed)
                checkingPositions.Add(_position);
            else if (_positionSource == PositionSource.FromData)
                checkingPositions.AddRange(Owner.GetData<GenerateOperationData>().NewBallsData.Select(i => i.GridPosition));

            var data = new CollapseOperationData();

            var maxDistanceToCheckingPosition = float.MinValue;
            foreach (var checkingPosition in checkingPositions)
            {
                var collapseLines = _field.CheckCollapse(checkingPosition);
                foreach (var collapseLine in collapseLines)
                {
                    _collapseLines.Add(new List<BallDesc>());
                    foreach (var ball in collapseLine)
                    {
                        _collapseLines[^1].Add(new BallDesc(ball.IntGridPosition, ball.Points, ball.HatName));
                        maxDistanceToCheckingPosition = Mathf.Max(maxDistanceToCheckingPosition, (ball.IntGridPosition - checkingPosition).magnitude);
                    }
                }
                
                foreach (var line in collapseLines)
                    foreach (var ball in line)
                        if(_ballsToRemove.FindIndex(i=>i.ball == ball) < 0)
                            _ballsToRemove.Add((ball, (ball.IntGridPosition - checkingPosition).magnitude));
            }

            
            data.CollapseLines = _collapseLines;
            Owner.SetData(data);

            var collapseLineWithResultPoints = _pointsCalculator.GetPoints(_collapseLines);

            var pointsGroups = new List<(List<(BallDesc ball, PointsDesc points)> ballPairs, Vector3 centerPosition)>();
            
            var sumPoints = 0;
            var groupCenterPosition = Vector3.zero;
            foreach (var collapseLine in collapseLineWithResultPoints)
            {
                var lineCenterPosition = Vector3.zero;
                foreach (var ballPair in collapseLine)
                {
                    lineCenterPosition += _field.GetPositionFromGrid(ballPair.ball.GridPosition) / collapseLine.Count;
                    sumPoints += ballPair.points.Sum();
                }

                groupCenterPosition += lineCenterPosition / collapseLineWithResultPoints.Count;
                pointsGroups.Add((collapseLine, lineCenterPosition));
            }
            _pointsAdded = sumPoints;
            _statisticsHolder.ChangeCollapseLinesCount(_collapseLines.Count);
            
            if (pointsGroups.Count > 0 && _pointsAdded > 0)
            {
                var findObjectOfType = GameObject.FindObjectOfType<UIFxLayer>();
                var collapsePointsEffect = Object.Instantiate(
                    _collapsePointsEffectPrefab, 
                    _field.View.Root.TransformPoint(groupCenterPosition), 
                    Quaternion.identity, 
                    findObjectOfType.transform);
                collapsePointsEffect.Run(pointsGroups);
            }
            
            var removeBallTasks = new List<Task>();
            foreach (var ballPair in _ballsToRemove)
                removeBallTasks.Add(RemoveBallWithDelay(ballPair.ball, ballPair.distance / maxDistanceToCheckingPosition, cancellationToken));
            
            await Task.WhenAll(removeBallTasks);
            
            return null;
        }

        private async Task RemoveBallWithDelay(Ball ball, float delay, CancellationToken cancellationToken)
        {
            await AsyncExtensions.WaitForSecondsAsync(delay * 0.15f, cancellationToken);
            _field.DestroyBalls(new List<Ball>(){ball}, false);
        }
        
        public override Operation GetInverseOperation()
        {
            return new UncollapseOperation(_collapseLines, _pointsAdded, _field, _statisticsHolder);
        }

        public enum PositionSource
        {
            Fixed,
            FromData,
        }
    }

    public class CollapseOperationData 
    {
        public List<List<BallDesc>> CollapseLines = new();
    }
}