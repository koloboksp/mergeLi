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
        private readonly IPointsChangeListener _pointsChangeListener;
        private readonly DestroyBallEffect _destroyBallEffectPrefab;
        private readonly CollapsePointsEffect _collapsePointsEffectPrefab;

        private readonly List<(Ball ball, float distance)> _ballsToRemove = new();
        private readonly List<List<BallDesc>> _collapseLines = new();
        private int _pointsAdded;

        public CollapseOperation(Vector3Int position, CollapsePointsEffect collapsePointsEffectPrefab,
            DestroyBallEffect destroyBallEffectPrefab, IField field, IPointsCalculator pointsCalculator, IPointsChangeListener pointsChangeListener)
        {
            _positionSource = PositionSource.Fixed;
            _position = position;
            _field = field;
            _pointsCalculator = pointsCalculator;
            _pointsChangeListener = pointsChangeListener;
            _collapsePointsEffectPrefab = collapsePointsEffectPrefab;
            _destroyBallEffectPrefab = destroyBallEffectPrefab;
        }

        public CollapseOperation(CollapsePointsEffect collapsePointsEffectPrefab, DestroyBallEffect destroyBallEffectPrefab, IField field, 
            IPointsCalculator pointsCalculator, IPointsChangeListener pointsChangeListener)
        {
            _positionSource = PositionSource.FromData;
            _field = field;
            _pointsCalculator = pointsCalculator;
            _pointsChangeListener = pointsChangeListener;
            _collapsePointsEffectPrefab = collapsePointsEffectPrefab;
            _destroyBallEffectPrefab = destroyBallEffectPrefab;
        }
    
        protected override async Task<object> InnerExecuteAsync(CancellationToken cancellationToken)
        {
            List<Vector3Int> checkingPositions = new List<Vector3Int>();
        
            if (_positionSource == PositionSource.Fixed)
                checkingPositions.Add(_position);
            else if (_positionSource == PositionSource.FromData)
                checkingPositions.AddRange(Owner.GetData<GenerateOperationData>().NewBallsData.Select(i => i.position));

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
                        _collapseLines[^1].Add(new BallDesc(ball.IntGridPosition, ball.Points));
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

           // foreach (var ballPair in _ballsToRemove)
           // {
           //     var destroyBallEffect = Object.Instantiate(
           //         _destroyBallEffectPrefab, 
           //         _field.View.Root.TransformPoint(_field.GetPositionFromGrid(ballPair.ball.IntGridPosition)), 
           //         Quaternion.identity, 
           //         _field.View.Root);
           //     
           //     destroyBallEffect.Run(ballPair.ball.GetColorIndex(), ballPair.distance / maxDistanceToCheckingPosition);
           // }

            var sumPoints = 0;
            foreach (var collapseLine in collapseLineWithResultPoints)
            {
                var groupsByPoints = collapseLine.GroupBy(i => i.Points);
                foreach (var groupByPoints in groupsByPoints)
                {
                    var valueTuples = groupByPoints.ToList();
                    var centerPosition = Vector3.zero;
                    var sumGroupPoints = 0;
                    foreach (var tuple in valueTuples)
                    {
                        centerPosition += _field.GetPositionFromGrid(tuple.GridPosition) / valueTuples.Count;
                        sumGroupPoints += tuple.Points;
                    }

                    var findObjectOfType = GameObject.FindObjectOfType<UIFxLayer>();
                    var collapsePointsEffect = Object.Instantiate(_collapsePointsEffectPrefab, 
                        _field.View.Root.TransformPoint(centerPosition), Quaternion.identity, 
                        _field.View.Root);
                    collapsePointsEffect.transform.SetParent(findObjectOfType.transform);
                    sumPoints += sumGroupPoints;
                    collapsePointsEffect.Run(sumGroupPoints, valueTuples.Count);
                }
            }

            var removeBallTasks = new List<Task>();
            foreach (var ballPair in _ballsToRemove)
                removeBallTasks.Add(RemoveBallWithDelay(ballPair.ball, ballPair.distance / maxDistanceToCheckingPosition, cancellationToken));
            
            await Task.WhenAll(removeBallTasks);
            
            //_field.DestroyBalls(_ballsToRemove.ConvertAll(i => i.ball));
            _pointsAdded = sumPoints;

            if(_pointsAdded != 0)
                _pointsChangeListener.AddPoints(_pointsAdded);

            return null;
        }

        private async Task RemoveBallWithDelay(Ball ball, float delay, CancellationToken cancellationToken)
        {
            await AsyncExtensions.WaitForSecondsAsync(delay * 0.15f, cancellationToken);
            _field.DestroyBalls(new List<Ball>(){ball}, false);
        }
        
        public override Operation GetInverseOperation()
        {
            return new UncollapseOperation(_collapseLines, _pointsAdded, _field, _pointsChangeListener);
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