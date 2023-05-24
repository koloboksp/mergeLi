using System.Collections.Generic;
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
        private readonly DestroyBallEffect _destroyBallEffectPrefab;
    
        private readonly List<Ball> _ballsToRemove = new List<Ball>();
        private readonly List<List<(Vector3Int, int)>> _collapseLines = new List<List<(Vector3Int, int)>>();
        public CollapseOperation(Vector3Int position, DestroyBallEffect destroyBallEffectPrefab, IField field)
        {
            _positionSource = PositionSource.Fixed;
            _position = position;
            _field = field;
            _destroyBallEffectPrefab = destroyBallEffectPrefab;
        }

        public CollapseOperation(DestroyBallEffect destroyBallEffectPrefab, IField field)
        {
            _positionSource = PositionSource.FromData;
            _field = field;
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
            
            foreach (var ball in _ballsToRemove)
            {
                var destroyBallEffect = Object.Instantiate(_destroyBallEffectPrefab, ball.transform.position, ball.transform.rotation, ball.transform.parent);
                destroyBallEffect.Run();
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