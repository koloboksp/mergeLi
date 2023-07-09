using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.Steps.CustomOperations
{
    public class SpendOperation : Operation
    {
        private readonly PlayerInfo _playerInfo;
        private readonly int _amount;
        private readonly bool _undoAvailable;
        
        public SpendOperation(int amount, PlayerInfo playerInfo, bool undoAvailable)
        {
            _amount = amount;
            _playerInfo = playerInfo;
            _undoAvailable = undoAvailable;
        }
        
        protected override void InnerExecute()
        {
            _playerInfo.ConsumeCoins(_amount);
            Complete(null);
        }

        public override Operation GetInverseOperation()
        {
            return _undoAvailable ? new SpendOperation(-_amount, _playerInfo, false) : null;
        }
    }
}