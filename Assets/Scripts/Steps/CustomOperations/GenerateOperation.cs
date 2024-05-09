using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Steps.CustomOperations
{
    public class GenerateOperation : Operation
    {
        private readonly Vector3Int _position;
        private readonly int _amount;
        private readonly int _maxAmount;
        private readonly int[] _availableValues;
        private readonly string[] _availableHats;
        private readonly IField _field;

        private readonly List<BallDesc> _generatedItems = new();
        
        public GenerateOperation(int amount, int maxAmount, int[] availableValues, string[] availableHats, IField field)
        {
            _amount = amount;
            _maxAmount = maxAmount;
            _availableValues = availableValues;
            _availableHats = availableHats;
            _field = field;
        }
    
        protected override async Task<object> InnerExecuteAsync(CancellationToken cancellationToken)
        {
            _generatedItems.AddRange(_field.GenerateBalls(_amount, _availableValues, _availableHats));
            _field.GenerateNextBallPositions(_maxAmount, _availableValues, _availableHats);
            Owner.SetData(new GenerateOperationData()
            {
                RequiredAmount = _amount,
                NewBallsData = _generatedItems
            });

            return null;
        }

        public override Operation GetInverseOperation()
        {
            return new RemoveGeneratedItemsOperation(_generatedItems.Select(i => i.GridPosition), _field);
        }
    }

    public class GenerateOperationData
    {
        public int RequiredAmount;
        public List<BallDesc> NewBallsData;
    }
}