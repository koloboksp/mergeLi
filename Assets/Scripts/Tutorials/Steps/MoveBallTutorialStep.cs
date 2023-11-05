using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Tutorials
{
    public class MoveBallTutorialStep : TutorialStep
    {
        [SerializeField] public Vector3Int _from;
        [SerializeField] public Vector3Int _to;

        protected override async Task<bool> InnerExecute(CancellationToken cancellationToken)
        {
            var balls = Tutorial.Controller.GameProcessor.GetField().GetSomething<Ball>(_to);
            if (balls.Count() > 0)
                await Tutorial.Controller.GameProcessor.MergeBall(_from, _to, cancellationToken);
            else
                await Tutorial.Controller.GameProcessor.MoveBall(_from, _to, cancellationToken);
             
            return true;
        }
    }
}