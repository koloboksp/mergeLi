using System;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class UIStartPanel : UIPanel
    {
        public event Action OnStartPlay;
        
        [SerializeField] private Button _closeBtn;
        [SerializeField] private Button _startBtn;

        private Model _model;
        private UIStartPanelData _data;

        private void Awake()
        {
            _closeBtn.onClick.AddListener(CloseBtn_OnClick);
            _startBtn.onClick.AddListener(StartBtn_OnClick);
        }

       
        private void CloseBtn_OnClick()
        {
           
        }
        
        private void StartBtn_OnClick()
        {
            ApplicationController.Instance.UIPanelController.PopScreen(this);
            OnStartPlay?.Invoke();
        }
        
        public override void SetData(UIScreenData undefinedData)
        {
            _data = undefinedData as UIStartPanelData;
            _model = new Model();
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