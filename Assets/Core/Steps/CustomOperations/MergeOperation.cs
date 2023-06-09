using System.Collections.Generic;
using System.Linq;
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
    
        protected override void InnerExecute()
        {
            var meargeables = _field.GetSomething<IFieldMergeable>(_position).ToList();
            var targetMergeable = meargeables[0];
            var otherMergeables = meargeables.GetRange(1, meargeables.Count - 1);

            _mergeablesCount = meargeables.Count;
            _pointsBeforeMerge = targetMergeable.Points;
            
            Owner.SetData(new MergeOperationData()
            {
                Position = _position,
                Points = _pointsBeforeMerge,
                MergeablesCount = _mergeablesCount,
            });
            
            targetMergeable.StartMerge(otherMergeables, OnMergeComplete);
        }
    
        private void OnMergeComplete(IFieldMergeable sender)
        {
            Complete(null);
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