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

            foreach (var checkingPosition in checkingPositions)
            {
                var collapseLines = _field.CheckCollapse(checkingPosition);
        
                foreach (var line in collapseLines)
                foreach (var ball in line)
                    if(!_ballsToRemove.Contains(ball))
                        _ballsToRemove.Add(ball);
            }
        
            OperationWaiter.WaitForSecond(_destroyBallEffectPrefab.Duration, Effect_OnComplete);
        
            foreach (var ball in _ballsToRemove)
            {
                var destroyBallEffect = Object.Instantiate(_destroyBallEffectPrefab, ball.transform.position, ball.transform.rotation, ball.transform.parent);
                destroyBallEffect.Run();
            }
        }

        private void Effect_OnComplete(OperationWaiter sender)
        {
            _field.DestroyBalls(_ballsToRemove);
        
            Complete(null);  
        }
    
        public enum PositionSource
        {
            Fixed,
            FromData,
        }
    }
}