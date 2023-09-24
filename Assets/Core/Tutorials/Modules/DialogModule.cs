using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Tutorials
{
    public class DialogModule : ModuleTutorialStep
    {
        [SerializeField] private List<string> _textKeys = new List<string>();
        
        public override async Task OnExecute(TutorialStep step, CancellationToken cancellationToken)
        {
            var focusedOnSomething = step as IFocusedOnSomething;
            if(focusedOnSomething != null)
                step.Tutorial.Controller.Dialog.Move(focusedOnSomething.GetFocusedRect());

            foreach (var textKey in _textKeys)
            {
                await step.Tutorial.Controller.Dialog.Show(textKey);
                await ApplicationController.WaitForSecondsAsync(1.0f, cancellationToken);
            }
        }

        public override async Task OnComplete(TutorialStep step, CancellationToken cancellationToken)
        {
            
        }

        public override void OnUpdate(TutorialStep step, CancellationToken cancellationToken)
        {
            
        }
    }
}