using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Tutorials
{
    public abstract class ModuleTutorialStep : MonoBehaviour
    {
        public abstract Task OnExecute(TutorialStep step, CancellationToken cancellationToken);
        public abstract Task OnComplete(TutorialStep step, CancellationToken cancellationToken);

        public abstract void OnUpdate(TutorialStep step, CancellationToken cancellationToken);
    }
}