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
            await InnerInitAsync(cancellationToken);
            
            await Task.WhenAll(gameObject.GetComponents<ModuleTutorialStep>()
                .Select(i=>i.OnExecuteAsync(this, cancellationToken))
                .ToArray());

            var result = await InnerExecuteAsync(cancellationToken);
            
            await Task.WhenAll(gameObject.GetComponents<ModuleTutorialStep>()
                .Select(i=>i.OnCompleteAsync(this, cancellationToken))
                .ToArray());

            return result;
        }

        protected virtual async Task<bool> InnerExecuteAsync(CancellationToken cancellationToken)
        {
            return true;
        }
        
        protected virtual async Task<bool> InnerInitAsync(CancellationToken cancellationToken)
        {
            return true;
        }
    }
}