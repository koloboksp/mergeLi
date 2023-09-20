using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Tutorials
{
    public class WaitForClickTutorialStep : TutorialStep
    {
        [SerializeField] private bool _waitForClick = false;
        protected override async Task<bool> InnerExecute(CancellationToken cancellationToken)
        {
            await Task.WhenAll(gameObject.GetComponents<ModuleTutorialStep>()
                .Select(i=>i.OnExecute(this))
                .ToArray());
            if(_waitForClick)
                await Tutorial.Controller.Focuser.WaitForClick(cancellationToken);
            gameObject.GetComponents<ModuleTutorialStep>().ToList().ForEach(i=>i.OnComplete(this));
            return true;
        }
    }
}