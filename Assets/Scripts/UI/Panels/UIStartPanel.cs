using System;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public enum UIStartPanelChoice
    {
        Continue,
        New
    }
    
    public class UIStartPanel : UIPanel
    {
        [SerializeField] private Button _closeBtn;
        [SerializeField] private Button _startBtn;
        [SerializeField] private Button _continueBtn;

        private Model _model;
        private UIStartPanelData _data;
        private UIStartPanelChoice _choice;
        
        public UIStartPanelChoice Choice => _choice;
        
        private void Awake()
        {
            _closeBtn.onClick.AddListener(CloseBtn_OnClick);
            _startBtn.onClick.AddListener(StartBtn_OnClick);
            _continueBtn.onClick.AddListener(ContinueBtn_OnClick);
        }
        
        private void CloseBtn_OnClick()
        {
            
        }
        
        private void StartBtn_OnClick()
        {
            _choice = UIStartPanelChoice.New;
            
            ApplicationController.Instance.UIPanelController.PopScreen(this);
        }
        
        private void ContinueBtn_OnClick()
        {
            _choice = UIStartPanelChoice.Continue;

            ApplicationController.Instance.UIPanelController.PopScreen(this);
        }
        
        public override void SetData(UIScreenData undefinedData)
        {
            _data = undefinedData as UIStartPanelData;
            _model = new Model();

            if (_data.GameProcessor.HasPreviousSessionGame)
            {
                _continueBtn.gameObject.SetActive(true);
            }
            else
            {
                _continueBtn.gameObject.SetActive(false);
            }
        }
        
        public class Model
        {
         
        }
    }
    
    public class UIStartPanelData : UIScreenData
    {
        public GameProcessor GameProcessor { get; set; }
    }
}