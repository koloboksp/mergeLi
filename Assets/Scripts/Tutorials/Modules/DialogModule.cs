using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Atom;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.Tutorials
{
    public class DialogModule : ModuleTutorialStep
    {
        [FormerlySerializedAs("_textKeysTemp")] [SerializeField] private GuidEx[] _textKeysT;
        [SerializeField] private DialogPosition _position = DialogPosition.Bottom;
        
        public override async Task OnExecute(TutorialStep step, CancellationToken cancellationToken)
        {
            step.Tutorial.Controller.Dialog.Move(_position);

            foreach (var textKey in _textKeysT)
            {
                await step.Tutorial.Controller.Dialog.ShowAsync(textKey, cancellationToken);
                await AsyncExtensions.WaitForSecondsAsync(1.0f, cancellationToken);
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