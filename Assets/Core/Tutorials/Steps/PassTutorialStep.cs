using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Tutorials
{
    public class PassTutorialStep : TutorialStep
    {
        [SerializeField] private float _wait;
        protected override async Task<bool> InnerExecute(CancellationToken cancellationToken)
        {
            if (_wait > 0)
                await AsyncExtensions.WaitForSecondsAsync(_wait, cancellationToken);
            return true;
        }
    }
}