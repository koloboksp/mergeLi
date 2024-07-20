using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Tutorials
{
    public class MoveFocusOnGridTutorialStep : TutorialStep, IFocusedOnSomething
    {
        [SerializeField] public Vector3Int _fromGridPosition;
        [SerializeField] public Vector3Int _toGridPosition;
        [SerializeField] public float _speed = 4.0f;

        private Rect _focusedRect;

        protected override async Task<bool> InnerExecuteAsync(CancellationToken cancellationToken)
        {
            var field = Tutorial.Controller.GameProcessor.Field;

            var from = _fromGridPosition;
            var to = _toGridPosition;
            var moveVector = to - from;

            var moveTime = moveVector.magnitude / _speed;
            var moveTimer = 0.0f;

            Tutorial.Controller.Focuser.gameObject.SetActive(true);

            var modules = gameObject.GetComponents<ModuleTutorialStep>();
            
            foreach (var module in modules)
                module.OnBeginUpdate(this);
            while (moveTimer < moveTime)
            {
                moveTimer += Time.deltaTime;
                
                var gridPosition = Vector3.Lerp(from, to, moveTimer / moveTime);

                var worldPosition = field.GetWorldPosition(gridPosition);
                var cellSize = field.GetWorldCellSize();
                _focusedRect = new Rect(worldPosition - cellSize * 2.5f * 0.5f, cellSize * 2.5f);
                foreach (var module in modules)
                    module.OnUpdate(this);
                
                await Task.Yield();
            }
            foreach (var module in modules)
                module.OnEndUpdate(this);
            
            return true;
        }

        public Rect GetFocusedRect()
        {
            return _focusedRect;
        }
    }
}