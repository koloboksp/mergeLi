using UnityEngine;
using UnityEngine.UI;

namespace UI.Utils.Console
{
    public class UIMiniAppConsole : MonoBehaviour
    {
        [SerializeField] private Button _openCloseAppConsoleBtn;
        [SerializeField] private Text _unreadInfoMessagesCountTxt;
        [SerializeField] private Text _unreadWarningMessagesCountTxt;
        [SerializeField] private Text _unreadErrorMessagesCountTxt;
        [SerializeField] private UIAppConsole _uiAppConsole;
    
        private volatile bool _unreadMessagesDirty;
    
        private int _unreadInfoMessagesCount;
        private int _unreadWarningMessagesCount;
        private int _unreadErrorMessagesCount;
    
        public void Awake()
        {
            _openCloseAppConsoleBtn.onClick.AddListener(OpenCloseAppConsoleBtn_OnClick);
            _uiAppConsole.OnUnreadMessagesCountChanged += AppConsole_OnUnreadMessagesCountChanged;
            AppConsole_OnUnreadMessagesCountChanged(_uiAppConsole);
        }
     
        private void OpenCloseAppConsoleBtn_OnClick()
        {
            _uiAppConsole.ChangeActiveState(!_uiAppConsole.ActiveState);
        }

        private void AppConsole_OnUnreadMessagesCountChanged(UIAppConsole sender)
        {         
            _unreadMessagesDirty = true;
            _unreadInfoMessagesCount = sender.UnreadInfoMessagesCount;
            _unreadWarningMessagesCount = sender.UnreadWarningMessagesCount;
            _unreadErrorMessagesCount = sender.UnreadErrorMessagesCount;          
        }

        private void Update()
        {  
            if (_unreadMessagesDirty)
            {
                _unreadMessagesDirty = false;

                _unreadInfoMessagesCountTxt.text = _unreadInfoMessagesCount.ToString();
                _unreadWarningMessagesCountTxt.text = _unreadWarningMessagesCount.ToString();
                _unreadErrorMessagesCountTxt.text = _unreadErrorMessagesCount.ToString();
            }
        }
    }
}