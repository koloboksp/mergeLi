using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.Steps.CustomOperations
{
    public class RemoveOperation : Operation
    {
        private readonly IField _field;
        private readonly List<Vector3Int> _indexes;

        private List<(Vector3Int, int)> _removedBalls;
        public RemoveOperation(IEnumerable<Vector3Int> indexes, IField field)
        {
            _indexes = new List<Vector3Int>(indexes);
            _field = field;
        }
        
        protected override void InnerExecute()
        {
            var foundBalls = _indexes.SelectMany(i => _field.GetSomething<Ball>(i)).ToList();
            _removedBalls = new List<(Vector3Int, int)>(foundBalls.Select(i => (i.IntPosition, i.Points)));
            _field.DestroyBalls(foundBalls);
            
            Complete(null);
        }

        public override Operation GetInverseOperation()
        {
            return new AddOperation(_removedBalls, _field);
        }
    }
}