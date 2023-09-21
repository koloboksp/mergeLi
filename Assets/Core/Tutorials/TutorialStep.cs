using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Tutorials
{
    public abstract class TutorialStep : MonoBehaviour
    {
        [SerializeField] private Tutorial _tutorial;

        public Tutorial Tutorial => _tutorial;
        public async Task<bool> Execute(CancellationToken cancellationToken)
        {
            await InnerInit(cancellationToken);
            
            await Task.WhenAll(gameObject.GetComponents<ModuleTutorialStep>()
                .Select(i=>i.OnExecute(this, cancellationToken))
                .ToArray());

            var result = await InnerExecute(cancellationToken);
            
            await Task.WhenAll(gameObject.GetComponents<ModuleTutorialStep>()
                .Select(i=>i.OnComplete(this, cancellationToken))
                .ToArray());

            return result;
        }

        protected virtual async Task<bool> InnerExecute(CancellationToken cancellationToken)
        {
            return true;
        }
        
        protected virtual async Task<bool> InnerInit(CancellationToken cancellationToken)
        {
            return true;
        }
    }
}