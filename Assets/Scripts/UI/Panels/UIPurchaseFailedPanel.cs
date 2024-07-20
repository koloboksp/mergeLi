using Core.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class UIPurchaseFailedPanel : UIPanel
    {
        [SerializeField] private Button _closeBtn;
        [SerializeField] private Button _continueBtn;
        
        private void Awake()
        {
            _closeBtn.onClick.AddListener(CloseBtn_OnClick);
            _continueBtn.onClick.AddListener(ContinueBtn_OnClick);
        }
        
        private void CloseBtn_OnClick()
        {
            ApplicationController.Instance.UIPanelController.PopScreen(this);
        }
        
        private void ContinueBtn_OnClick()
        {
            ApplicationController.Instance.UIPanelController.PopScreen(this);
        }
        
        public override void SetData(UIScreenData undefinedData)
        {
            base.SetData(undefinedData);

            var data = undefinedData as UIPurchaseFailedPanelData;
        }
        
        public class Model
        {
         
        }
    }
    
    public class UIPurchaseFailedPanelData : UIScreenData
    {
        public GameProcessor GameProcessor { get; set; }
    }
}