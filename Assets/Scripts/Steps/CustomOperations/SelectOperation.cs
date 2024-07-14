using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Gameplay;
using UnityEngine;

namespace Core.Steps.CustomOperations
{
    public class SelectOperation : Operation
    {
        private readonly Vector3Int _position;
        private readonly bool _selectState;
        private readonly IField _field;
        
        public SelectOperation(Vector3Int position, bool selectState, IField field)
        {
            _position = position;
            _selectState = selectState;
            _field = field;
        }

        protected override async Task<object> InnerExecuteAsync(CancellationToken cancellationToken)
        {
            var selectables = _field.GetSomething<IFieldSelectable>(_position).ToList();
            var firstSelectable = selectables[0];
            firstSelectable.Select(_selectState);

            return firstSelectable;   
        }
    }
}