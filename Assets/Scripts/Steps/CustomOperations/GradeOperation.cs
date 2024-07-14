using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Gameplay;
using UnityEngine;

namespace Core.Steps.CustomOperations
{
    public class GradeOperation : Operation
    {
        private readonly IField _field;
        private readonly List<Vector3Int> _indexes;
        private readonly int _level;

       
        public GradeOperation(IEnumerable<Vector3Int> indexes, int level, IField field)
        {
            _indexes = new List<Vector3Int>(indexes);
            _level = level;
            _field = field;
        }
        
        protected override async Task<object> InnerExecuteAsync(CancellationToken cancellationToken)
        {
            var foundBalls = _indexes.SelectMany(i => _field.GetSomething<Ball>(i)).ToList();
            foreach (var foundBall in foundBalls)
                await foundBall.StartGrade(_level, cancellationToken);

            return null;
        }
        
        public override Operation GetInverseOperation()
        {
            return new GradeOperation(_indexes, -_level, _field);
        }
    }
}