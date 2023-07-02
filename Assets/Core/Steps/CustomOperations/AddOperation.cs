using System.Collections.Generic;
using UnityEngine;

namespace Core.Steps.CustomOperations
{
    public class AddOperation : Operation
    {
        private readonly IField _field;
        private readonly List<(Vector3Int index, int points)> _ballsToAdd;

        public AddOperation(IEnumerable<(Vector3Int index, int points)> ballsToAdd, IField field)
        {
            _ballsToAdd = new List<(Vector3Int index, int points)>(ballsToAdd);
            _field = field;
        }
        
        protected override void InnerExecute()
        {
            foreach (var ballToAdd in _ballsToAdd)
                _field.CreateBall(ballToAdd.index, ballToAdd.points);
            
            Complete(null);
        }
    }
}