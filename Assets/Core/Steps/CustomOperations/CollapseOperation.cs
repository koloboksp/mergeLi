using System.Collections.Generic;
using System.Linq;
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
        private readonly DestroyBallEffect _destroyBallEffectPrefab;
        private readonly CollapsePointsEffect _collapsePointsEffectPrefab;

        private readonly List<Ball> _ballsToRemove = new List<Ball>();
        private readonly List<List<(Vector3Int position, int points)>> _collapseLines = new List<List<(Vector3Int, int)>>();
        
        public CollapseOperation(Vector3Int position, CollapsePointsEffect collapsePointsEffectPrefab,
            DestroyBallEffect destroyBallEffectPrefab, IField field, IPointsCalculator pointsCalculator)
        {
            _positionSource = PositionSource.Fixed;
            _position = position;
            _field = field;
            _pointsCalculator = pointsCalculator;
            _collapsePointsEffectPrefab = collapsePointsEffectPrefab;
            _destroyBallEffectPrefab = destroyBallEffectPrefab;
        }

        public CollapseOperation(CollapsePointsEffect collapsePointsEffectPrefab, DestroyBallEffect destroyBallEffectPrefab, IField field, IPointsCalculator pointsCalculator)
        {
            _positionSource = PositionSource.FromData;
            _field = field;
            _pointsCalculator = pointsCalculator;
            _collapsePointsEffectPrefab = collapsePointsEffectPrefab;
            _destroyBallEffectPrefab = destroyBallEffectPrefab;
        }
    
        protected override void InnerExecute()
        {
            List<Vector3Int> checkingPositions = new List<Vector3Int>();
        
            if (_positionSource == PositionSource.Fixed)
                checkingPositions.Add(_position);
            else if (_positionSource == PositionSource.FromData)
                checkingPositions.AddRange(Owner.GetData<GenerateOperationData>().NewPositions);

            var data = new CollapseOperationData();
            
            foreach (var checkingPosition in checkingPositions)
            {
                List<List<Ball>> collapseLines = _field.CheckCollapse(checkingPosition);
                foreach (var collapseLine in collapseLines)
                {
                    _collapseLines.Add(new List<(Vector3Int, int)>());
                    foreach (var ball in collapseLine)
                        _collapseLines[_collapseLines.Count-1].Add((ball.IntPosition, ball.Points));
                }
           
                foreach (var line in collapseLines)
                    foreach (var ball in line)
                        if(!_ballsToRemove.Contains(ball))
                            _ballsToRemove.Add(ball);
            }

            
            data.CollapseLines = _collapseLines;
            Owner.SetData(data);

            var collapseLineWithResultPoints = _pointsCalculator.GetPoints(_collapseLines);

            foreach (var ball in _ballsToRemove)
            {
                var destroyBallEffect = Object.Instantiate(_destroyBallEffectPrefab, 
                    _field.FieldView.FieldRoot.TransformPoint(_field.GetPosition(ball.IntPosition)), Quaternion.identity, 
                    _field.FieldView.FieldRoot);
                destroyBallEffect.Run();
            }
            
            foreach (var collapseLine in collapseLineWithResultPoints)
            {
                var groupsByPoints = collapseLine.GroupBy(i => i.points);
                foreach (var groupByPoints in groupsByPoints)
                {
                    var valueTuples = groupByPoints.ToList();
                    var centerPosition = Vector3.zero;
                    var sumPoints = 0;
                    foreach (var tuple in valueTuples)
                    {
                        centerPosition += _field.GetPosition(tuple.position) / valueTuples.Count;
                        sumPoints += tuple.points;
                    }
                    
                    var collapsePointsEffect = Object.Instantiate(_collapsePointsEffectPrefab, 
                        _field.FieldView.FieldRoot.TransformPoint(centerPosition), Quaternion.identity, 
                        _field.FieldView.FieldRoot);
                    
                    collapsePointsEffect.Run(sumPoints);
                }
            }
 
            _field.DestroyBalls(_ballsToRemove);
            Complete(null); 
        }

        public override Operation GetInverseOperation()
        {
            return new UncollapseOperation(_collapseLines, _field);
        }

        public enum PositionSource
        {
            Fixed,
            FromData,
        }
    }

    public class CollapseOperationData 
    {
        public List<List<(Vector3Int position, int points)>> CollapseLines = new();
    }
}