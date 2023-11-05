using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.Steps.CustomOperations
{
    public class GenerateOperation : Operation
    {
        private readonly Vector3Int _position;
        private readonly int _amount;
        private readonly int _maxAmount;
        private readonly List<int> _availableValues;
        private readonly IField _field;

        private readonly List<(Vector3Int position, int points)> _generatedItems = new();
        
        public GenerateOperation(int amount, int maxAmount, List<int> availableValues, IField field)
        {
            _amount = amount;
            _maxAmount = maxAmount;
            _availableValues = availableValues;
            _field = field;
        }
    
        protected override void InnerExecute()
        {
            _generatedItems.AddRange(_field.GenerateBalls(_amount, _availableValues));
            _field.GenerateNextBallPositions(_maxAmount, _availableValues);
            Owner.SetData(new GenerateOperationData()
            {
                RequiredAmount = _amount,
                NewBallsData = _generatedItems
            });

            Complete(null);
        }

        public override Operation GetInverseOperation()
        {
            return new RemoveGeneratedItemsOperation(_generatedItems.Select(i => i.position), _field);
        }
    }

    public class GenerateOperationData
    {
        public int RequiredAmount;
        public List<(Vector3Int position, int points)> NewBallsData;
    }
}