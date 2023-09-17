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
            return await InnerExecute(cancellationToken);
        }

        protected virtual async Task<bool> InnerExecute(CancellationToken cancellationToken)
        {
            return true;
        }
    }
}