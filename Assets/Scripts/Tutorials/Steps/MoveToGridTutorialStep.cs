using UnityEngine;

namespace Core.Tutorials
{
    public class MoveToGridTutorialStep : BaseMoveToTutorialStep
    {
        [SerializeField] public Vector3Int _toGridPosition;
        
        protected override (Vector2 position, Vector2 size) GetToPose()
        {
            var field = Tutorial.Controller.GameProcessor.Field;
            var cellSize = field.GetWorldCellSize();
            var scaledCellSize = cellSize * 2.5f;
            var toMin = field.GetWorldPosition(_toGridPosition) - scaledCellSize * 0.5f;
            var toSize = scaledCellSize;

            return (toMin, toSize);
        }
    }
}