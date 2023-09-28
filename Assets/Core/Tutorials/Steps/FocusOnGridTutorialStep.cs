using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.Tutorials
{
    public class FocusOnGridTutorialStep : TutorialStep, IFocusedOnSomething
    {
        [SerializeField] public Vector3Int _gridPosition;
       
        private float _rectScale = 2.5f;
        private Rect _focusedRect;

        protected override async Task<bool> InnerInit(CancellationToken cancellationToken)
        {
            var worldPosition = Tutorial.Controller.GameProcessor.GetField().GetWorldPosition(_gridPosition);
            var cellSize = Tutorial.Controller.GameProcessor.GetField().GetWorldCellSize();
            _focusedRect = new Rect(worldPosition - cellSize * _rectScale * 0.5f, cellSize * _rectScale);
            
            return true;
        }

        protected override async Task<bool> InnerExecute(CancellationToken cancellationToken)
        {
            return true;
        }
        
        public Rect GetFocusedRect()
        {
            return _focusedRect;
        }
    }
}