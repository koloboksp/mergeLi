using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public sealed class WaitForButtonClick : CustomYieldInstruction
    {
        private readonly Button _button;
        private bool _keepWaiting = true;
        public override bool keepWaiting
        {
            get
            {
                return _keepWaiting;
            }
        }

        public WaitForButtonClick(Button button)
        {
            _button = button;
            _button.onClick.AddListener(OnButtonClick);
        }

        void OnButtonClick()
        {
            _keepWaiting = false;
        }
    }
}