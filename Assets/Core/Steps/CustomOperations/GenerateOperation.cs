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

        public GenerateOperation(int count, IField field)
        {
            _count = count;
            _field = field;
        }
    
        protected override void InnerExecute()
        {
            List<Vector3Int> generateBalls = _field.GenerateBalls(_count);
            Owner.SetData(new GenerateOperationData(){ NewPositions = generateBalls });

            OperationWaiter.WaitForSecond(1, Effect_OnComplete);
        }

        private void Effect_OnComplete(OperationWaiter sender)
        {
            Complete(null);
        }
    }

    public class GenerateOperationData
    {
        public List<Vector3Int> NewPositions;
    }
}