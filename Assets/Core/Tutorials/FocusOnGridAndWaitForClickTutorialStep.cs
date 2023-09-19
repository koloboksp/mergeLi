using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.Tutorials
{
    public class FocusOnGridAndWaitForClickTutorialStep : TutorialStep, IFocusedOnSomething
    {
        [SerializeField] public Vector3Int _gridPosition;
        
        private Rect _focusedRect;
        
        protected override async Task<bool> InnerExecute(CancellationToken cancellationToken)
        {
            Tutorial.Controller.Focuser.gameObject.SetActive(true);
  
            var worldPosition = Tutorial.Controller.GameProcessor.GetField().GetWorldPosition(_gridPosition);
            var cellSize = Tutorial.Controller.GameProcessor.GetField().GetWorldCellSize();
            _focusedRect = new Rect(worldPosition - cellSize * 2.5f * 0.5f, cellSize * 2.5f);

            gameObject.GetComponents<ModuleTutorialStep>().ToList().ForEach(i=>i.OnExecute(this));
            Tutorial.Controller.Focuser.FocusOn(_focusedRect);

            await Tutorial.Controller.Focuser.WaitForClick(cancellationToken);
            
            gameObject.GetComponents<ModuleTutorialStep>().ToList().ForEach(i=>i.OnComplete(this));
            return true;
        }
        
        public Rect GetFocusedRect()
        {
            return _focusedRect;
        }
    }
}