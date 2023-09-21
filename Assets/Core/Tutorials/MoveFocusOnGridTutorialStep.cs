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
        
        protected override async Task<bool> InnerExecute(CancellationToken cancellationToken)
        {
            var field = Tutorial.Controller.GameProcessor.GetField();
            
            Vector3 from = _fromGridPosition;
            Vector3 to = _toGridPosition;
            Vector3 moveVector = _toGridPosition - _fromGridPosition;
            
            var moveTime = moveVector.magnitude / _speed;
            var moveTimer = 0.0f;
            
            Tutorial.Controller.Focuser.gameObject.SetActive(true);

            while (moveTimer < moveTime)
            {
                var gridPosition = Vector3.Lerp(from, to, moveTimer / moveTime);
              
                var worldPosition = field.GetWorldPosition(gridPosition);
                var cellSize = field.GetWorldCellSize();
                _focusedRect = new Rect(worldPosition - cellSize * 2.5f * 0.5f, cellSize * 2.5f);
                gameObject.GetComponents<ModuleTutorialStep>().ToList().ForEach(i=>i.OnUpdate(this, cancellationToken));
                
                moveTimer += Time.deltaTime;
                await Task.Yield();
            }
            
            return true;
        }
        
        public Rect GetFocusedRect()
        {
            return _focusedRect;
        }
    }
}