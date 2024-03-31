using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class UIConfirmRestartPanel : UIPanel
    {
        [SerializeField] private Button _closeBtn;
        [SerializeField] private Button _yesBtn;
        [SerializeField] private Button _noBtn;

        private bool _isConfirmed;
        public bool IsConfirmed => _isConfirmed;
        
        private void Awake()
        {
            _closeBtn.onClick.AddListener(CloseBtn_OnClick);
            
            _yesBtn.onClick.AddListener(YesBtn_OnClick);
            _noBtn.onClick.AddListener(NoBtn_OnClick);
        }
        
        private void CloseBtn_OnClick()
        {
            _isConfirmed = false;
            ApplicationController.Instance.UIPanelController.PopScreen(this);
        }
        
        private void YesBtn_OnClick()
        {
            _isConfirmed = true;
            ApplicationController.Instance.UIPanelController.PopScreen(this);
        }
        
        private void NoBtn_OnClick()
        {
            _isConfirmed = false;
            ApplicationController.Instance.UIPanelController.PopScreen(this);
        }
    }
}