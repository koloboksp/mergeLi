using System.Linq;
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

        protected override void InnerExecute()
        {
            var selectables = _field.GetSomething<IFieldSelectable>(_position).ToList();
            var selectable = selectables[0];
            selectable.Select(_selectState);

            Complete(selectable);   
        }
    }
}