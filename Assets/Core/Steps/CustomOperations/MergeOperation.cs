using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Steps.CustomOperations
{
    public class MergeOperation : Operation
    {
        private readonly Vector3Int _position;
        private readonly IField _field;

        private int _pointsBeforeMerge;
        private int _mergeablesCount;
        public MergeOperation(Vector3Int position, IField field)
        {
            _position = position;
            _field = field;
        }
    
        protected override async Task<object> InnerExecuteAsync(CancellationToken cancellationToken)
        {
            var meargeables = _field.GetSomething<IFieldMergeable>(_position).ToList();
            var targetMergeable = meargeables[0];
            var otherMergeables = meargeables.GetRange(1, meargeables.Count - 1);

            _mergeablesCount = meargeables.Count;
            if(targetMergeable is IBall targetBall)
                _pointsBeforeMerge = targetBall.Points;
            
            Owner.SetData(new MergeOperationData()
            {
                Position = _position,
                Points = _pointsBeforeMerge,
                MergeablesCount = _mergeablesCount,
            });
            
            await targetMergeable.MergeAsync(otherMergeables, cancellationToken);

            return null;
        }
        
        public override Operation GetInverseOperation()
        {
            return new UnmergeOperation(_position, _pointsBeforeMerge, _mergeablesCount, _field);
        }
    }
    
    public class MergeOperationData
    {
        public Vector3Int Position;
        public int Points;
        public int MergeablesCount;
    }
}