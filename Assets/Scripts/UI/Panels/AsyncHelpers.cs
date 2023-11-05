using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace Core
{
    public static class AsyncHelpers
    {
        public static async Task WaitForClick(Button button, CancellationToken cancellationToken)
        {
            button.onClick.AddListener(OnClick);
            var buttonClicked = false;
            
            while (!buttonClicked)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    button.onClick.RemoveListener(OnClick);
                    throw new OperationCanceledException();
                }

                await Task.Yield();
            }

            return;

            void OnClick()
            {
                button.onClick.RemoveListener(OnClick);
                buttonClicked = true;
            }
        }
    }
}