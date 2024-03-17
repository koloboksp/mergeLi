using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Tutorials
{
    public abstract class ModuleTutorialStep : MonoBehaviour
    {
        public virtual async Task OnExecuteAsync(TutorialStep step, CancellationToken cancellationToken)
        {
            
        }

        public virtual async Task OnCompleteAsync(TutorialStep step, CancellationToken cancellationToken)
        {
           
        }

        public virtual void OnUpdate(TutorialStep step)
        {
            
        }
    }
}