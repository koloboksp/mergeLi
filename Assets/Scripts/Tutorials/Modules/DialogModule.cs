﻿using System.Collections.Generic;
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
        [SerializeField] private bool _tapRequired = true;

        public override async Task OnExecuteAsync(TutorialStep step, CancellationToken cancellationToken)
        {
            step.Tutorial.Controller.Dialog.Move(_position);

            foreach (var textKey in _textKeysT)
            {
                await step.Tutorial.Controller.Dialog.ShowAsync(textKey, _tapRequired, cancellationToken);
            }
        }

        public override async Task OnCompleteAsync(TutorialStep step, CancellationToken cancellationToken)
        {
            
        }
    }
}