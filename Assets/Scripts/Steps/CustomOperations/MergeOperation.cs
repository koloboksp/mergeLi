using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Gameplay;
using UnityEngine;

namespace Core.Steps.CustomOperations
{
    public class MergeOperation : Operation
    {
        private readonly Vector3Int _position;
        private readonly IField _field;

        //private int _pointsBeforeMerge;
        //private int _mergeablesCount;
        private readonly List<BallDesc> _mergedBalls = new List<BallDesc>();
        
        public MergeOperation(Vector3Int position, IField field)
        {
            _position = position;
            _field = field;
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
            
            await targetMergeable.MergeAsync(otherMergeables, cancellationToken);

            return null;
        }
        
        public override Operation GetInverseOperation()
        {
            return new UnmergeOperation(_mergedBalls, _field);
        }
    }
    
    public class MergeOperationData
    {
        public List<BallDesc> MergedBalls;
    }
}