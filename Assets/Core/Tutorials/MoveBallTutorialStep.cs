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
            await Task.WhenAll(gameObject.GetComponents<ModuleTutorialStep>()
                .Select(i=>i.OnExecute(this, cancellationToken))
                .ToArray());
            
            await Tutorial.Controller.GameProcessor.MoveBall(_from, _to, cancellationToken);
            
            return true;
        }
    }
}