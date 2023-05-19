using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.Steps.CustomOperations
{
    public class MergeOperation : Operation
    {
        private readonly Vector3Int _position;
        private readonly IField _field;

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
            
            Owner.SetData(new MergeOperationData()
            {
                Position = _position,
                Points = targetMergeable.Points,
                MergeablesCount = meargeables.Count,
            });
            
            targetMergeable.StartMerge(otherMergeables, OnMergeComplete);
        }
    
        private void OnMergeComplete(IFieldMergeable sender)
        {
            Complete(null);
        }
    }
    
    public class MergeOperationData
    {
        public Vector3Int Position;
        public int Points;
        public int MergeablesCount;
    }
}