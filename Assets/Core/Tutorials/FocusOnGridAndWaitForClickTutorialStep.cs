using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.Tutorials
{
    public class FocusOnGridAndWaitForClickTutorialStep : TutorialStep
    {
        [FormerlySerializedAs("_ballPosition")] [SerializeField] public Vector3Int _gridPosition;
        
        protected override async Task<bool> InnerExecute(CancellationToken cancellationToken)
        {
            Tutorial.Controller.Focuser.gameObject.SetActive(true);
            
           // var balls = Tutorial.Controller.GameProcessor.GetField().GetSomething<Ball>(_gridPosition);
           // var ball = balls.FirstOrDefault();
           // if (ball != null)
           // {
           //     Tutorial.Controller.Focuser.FocusOn(ball.View.Root);
           // }
           // else
            {
                var worldPosition = Tutorial.Controller.GameProcessor.GetField().GetWorldPosition(_gridPosition);
                var worldPosition1 = Tutorial.Controller.GameProcessor.GetField().GetWorldPosition(_gridPosition + new Vector3Int(1,1,0));
                var cellSize = worldPosition1 - worldPosition;
                var rect = new Rect(worldPosition - cellSize * 2.5f * 0.5f, cellSize * 2.5f);
                Tutorial.Controller.Focuser.FocusOn(rect);
            }
            
            await Tutorial.Controller.Focuser.WaitForClick(cancellationToken);
            return true;
        }
    }
}