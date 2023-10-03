using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.Steps.CustomOperations
{
    public class SpendOperation : Operation
    {
        private readonly GameProcessor _gameProcessor;
        private readonly int _amount;
        private readonly bool _undoAvailable;
        
        public SpendOperation(int amount, GameProcessor gameProcessor, bool undoAvailable)
        {
            _amount = amount;
            _gameProcessor = gameProcessor;
            _undoAvailable = undoAvailable;
        }
        
        protected override void InnerExecute()
        {
            _gameProcessor.ConsumeCoins(_amount);
            Complete(null);
        }

        public override Operation GetInverseOperation()
        {
            return _undoAvailable ? new SpendOperation(-_amount, _gameProcessor, false) : null;
        }
    }
}