using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Effects;
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

        private readonly List<(Ball ball, float distance)> _ballsToRemove = new();
        private readonly List<List<BallDesc>> _collapseLines = new();
        private int _pointsAdded;

        public CollapseOperation(
            Vector3Int position, 
            CollapsePointsEffect collapsePointsEffectPrefab,
            IField field, 
            IPointsCalculator pointsCalculator)
        {
            _positionSource = PositionSource.Fixed;
            _position = position;
            _field = field;
            _pointsCalculator = pointsCalculator;
            _collapsePointsEffectPrefab = collapsePointsEffectPrefab;
        }

        public CollapseOperation(
            CollapsePointsEffect collapsePointsEffectPrefab,
            IField field, 
            IPointsCalculator pointsCalculator)
        {
            _positionSource = PositionSource.FromData;
            _field = field;
            _pointsCalculator = pointsCalculator;
            _collapsePointsEffectPrefab = collapsePointsEffectPrefab;
        }
    
        protected override async Task<object> InnerExecuteAsync(CancellationToken cancellationToken)
        {
            List<Vector3Int> checkingPositions = new List<Vector3Int>();
        
            if (_positionSource == PositionSource.Fixed)
                checkingPositions.Add(_position);
            else if (_positionSource == PositionSource.FromData)
                checkingPositions.AddRange(Owner.GetData<GenerateOperationData>().NewBallsData.Select(i => i.GridPosition));

            var data = new CollapseOperationData();

            var maxDistanceToCheckingPosition = float.MinValue;
            foreach (var checkingPosition in checkingPositions)
            {
                List<List<Ball>> collapseLines = _field.CheckCollapse(checkingPosition);
                foreach (var collapseLine in collapseLines)
                {
                    _collapseLines.Add(new List<BallDesc>());
                    foreach (var ball in collapseLine)
                    {
                        _collapseLines[^1].Add(new BallDesc(ball.IntGridPosition, ball.Points, ball.Hat));
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

            var pointsGroups = new List<(int points, int ballsCount, Vector3 position)>();
            
            var sumPoints = 0;
            foreach (var collapseLine in collapseLineWithResultPoints)
            {
                var groupsByPoints = collapseLine.GroupBy(i => i.Points);
                foreach (var groupByPoints in groupsByPoints)
                {
                    var ballsInGroup = groupByPoints.Count();
                    
                    var centerPosition = Vector3.zero;
                    var sumGroupPoints = 0;
                    foreach (var ballDesc in groupByPoints)
                    {
                        centerPosition += _field.GetPositionFromGrid(ballDesc.GridPosition) / ballsInGroup;
                        sumGroupPoints += ballDesc.Points;
                    }

                    pointsGroups.Add((sumGroupPoints, ballsInGroup, _field.View.Root.TransformPoint(centerPosition)));
                    
                    sumPoints += sumGroupPoints;
                }
            }

            if (pointsGroups.Count > 0)
            {
                var findObjectOfType = GameObject.FindObjectOfType<UIFxLayer>();
                var collapsePointsEffect = Object.Instantiate(_collapsePointsEffectPrefab, 
                    _field.View.Root.TransformPoint(pointsGroups[0].position), Quaternion.identity, 
                    _field.View.Root);
                collapsePointsEffect.transform.SetParent(findObjectOfType.transform);
                collapsePointsEffect.Run(pointsGroups);
            }
            
            var removeBallTasks = new List<Task>();
            foreach (var ballPair in _ballsToRemove)
                removeBallTasks.Add(RemoveBallWithDelay(ballPair.ball, ballPair.distance / maxDistanceToCheckingPosition, cancellationToken));
            
            await Task.WhenAll(removeBallTasks);
            
            _pointsAdded = sumPoints;
            
            return null;
        }

        private async Task RemoveBallWithDelay(Ball ball, float delay, CancellationToken cancellationToken)
        {
            await AsyncExtensions.WaitForSecondsAsync(delay * 0.15f, cancellationToken);
            _field.DestroyBalls(new List<Ball>(){ball}, false);
        }
        
        public override Operation GetInverseOperation()
        {
            return new UncollapseOperation(_collapseLines, _pointsAdded, _field);
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