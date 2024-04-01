using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Common
{
    public class UIExtendedButton : Button
    {
        private UnityEvent _onPressed = new UnityEvent();
        private UnityEvent _onClickFail = new UnityEvent();

        public UnityEvent onPressed
        {
            get { return _onPressed; }
            set { _onPressed = value; }
        }
        
        public UnityEvent onClickFail
        {
            get { return _onClickFail; }
            set { _onClickFail = value; }
        }
        
        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);

            if (IsPressed())
            {
                _onPressed.Invoke();
            }
            else
            {
                _onClickFail.Invoke();
            }
        }
    }
}