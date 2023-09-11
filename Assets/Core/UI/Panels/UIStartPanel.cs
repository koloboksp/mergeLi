using System;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class UIStartPanel : UIPanel
    {
        public event Action OnStartPlay;
        public event Action OnContinuePlay;

        [SerializeField] private Button _closeBtn;
        [SerializeField] private Button _startBtn;
        [SerializeField] private Button _continueBtn;

        private Model _model;
        private UIStartPanelData _data;

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
            ApplicationController.Instance.UIPanelController.PopScreen(this);
            OnStartPlay?.Invoke();
        }
        
        private void ContinueBtn_OnClick()
        {
            ApplicationController.Instance.UIPanelController.PopScreen(this);
            OnContinuePlay?.Invoke();
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