using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Effects;
using Core.Gameplay;
using UnityEngine;

namespace Core.Steps.CustomOperations
{
    public class MergeOperation : Operation
    {
        private readonly Vector3Int _position;
        private readonly IField _field;
        private readonly ISessionStatisticsHolder _statisticsHolder;
        private readonly MergeBallsEffect _mergeBallsEffectPrefab;

        private readonly List<BallDesc> _mergedBalls = new();
        
        public MergeOperation(
            Vector3Int position, 
            IField field, 
            ISessionStatisticsHolder statisticsHolder,
            MergeBallsEffect mergeBallsEffectPrefab)
        {
            _position = position;
            _field = field;
            _statisticsHolder = statisticsHolder;
            _mergeBallsEffectPrefab = mergeBallsEffectPrefab;
        }
    
        protected override async Task<object> InnerExecuteAsync(CancellationToken cancellationToken)
        {
            var meargeables = _field.GetSomething<IBall>(_position).ToList();
            var targetMergeable = meargeables[0];
            var otherMergeables = meargeables.GetRange(1, meargeables.Count - 1);
            
            foreach (var ball in meargeables)
                _mergedBalls.Add(new BallDesc(ball.IntGridPosition, ball.Points, ball.HatName));
                
            Owner.SetData(new MergeOperationData()
            {
                MergedBalls = new List<BallDesc>(_mergedBalls)
            });
    
            var mergeblePosition = _field.GetPositionFromGrid(targetMergeable.IntGridPosition);

            var findObjectOfType = GameObject.FindObjectOfType<UIFxLayer>();
            var mergeBallsEffect = Object.Instantiate(
                _mergeBallsEffectPrefab, 
                _field.View.Root.TransformPoint(mergeblePosition), 
                Quaternion.identity, 
                findObjectOfType.transform);
            mergeBallsEffect.Run();
            
            await targetMergeable.MergeAsync(otherMergeables, cancellationToken);
            _statisticsHolder.ChangeMergeCount(1);
            
            return null;
        }
        
        public override Operation GetInverseOperation()
        {
            return new UnmergeOperation(_mergedBalls, _field, _statisticsHolder);
        }
    }
    
    public class MergeOperationData
    {
        public List<BallDesc> MergedBalls;
    }
}