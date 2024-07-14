using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Gameplay;
using UnityEngine;

namespace Core.Steps.CustomOperations
{
    public class AddOperation : Operation
    {
        private readonly IField _field;
        private readonly List<BallDesc> _ballsToAdd;

        public AddOperation(IEnumerable<BallDesc> ballsToAdd, IField field)
        {
            _ballsToAdd = new List<BallDesc>(ballsToAdd);
            _field = field;
        }
        
        protected override async Task<object> InnerExecuteAsync(CancellationToken cancellationToken)
        {
            foreach (var ballData in _ballsToAdd)
                _field.CreateBall(ballData.GridPosition, ballData.Points, ballData.HatName);

            return null;
        }
    }
}