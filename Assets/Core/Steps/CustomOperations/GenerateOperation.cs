using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.Steps.CustomOperations
{
    public class GenerateOperation : Operation
    {
        private readonly Vector3Int _position;
        private readonly int _count;
        private readonly IField _field;

        private readonly List<Vector3Int> _generatedItems = new List<Vector3Int>();
        
        public GenerateOperation(int count, IField field)
        {
            _count = count;
            _field = field;
        }
    
        protected override void InnerExecute()
        {
            _generatedItems.AddRange(_field.GenerateBalls(_count));
            Owner.SetData(new GenerateOperationData(){ NewPositions = _generatedItems});

            Complete(null);
        }

        public override Operation GetInverseOperation()
        {
            return new RemoveGeneratedItemsOperation(_generatedItems, _field);
        }
    }

    public class GenerateOperationData
    {
        public List<Vector3Int> NewPositions;
    }
}