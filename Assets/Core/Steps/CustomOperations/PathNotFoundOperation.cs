using UnityEngine;

namespace Core.Steps.CustomOperations
{
    public class PathNotFoundOperation : Operation
    {
        private Vector3Int _endPosition;
        private readonly IField _field;
        private readonly NoPathEffect _noPathEffect;

        public PathNotFoundOperation(Vector3Int endPosition, NoPathEffect noPathEffect, IField field)
        {
            _endPosition = endPosition;
            _noPathEffect = noPathEffect;
            _field = field;
        }

        protected override void InnerExecute()
        {
            var position = _field.FieldView.FieldRoot.TransformPoint(_field.GetPosition(_endPosition));
            var noPathEffect = Object.Instantiate(_noPathEffect, position, Quaternion.identity, _field.FieldView.FieldRoot);
            noPathEffect.Run();
            
            Complete(null);
        }
    }
}