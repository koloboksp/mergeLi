using System;
using Assets.Scripts.Core;
using Atom;
using UnityEngine;
using UnityEngine.Serialization;
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
        [SerializeField] private Button _playBtn;
        [SerializeField] private UIStaticTextLocalizator _playLabelBtn;
       
        [SerializeField] private GuidEx _playLocKey;
        [SerializeField] private GuidEx _continueLocKey;

        [SerializeField] private SpawnAnimator _panelAnimator;
        private Model _model;
        private UIStartPanelData _data;
        private UIStartPanelChoice _choice;
        
        public UIStartPanelChoice Choice => _choice;
        
        private void Awake()
        {
            _playBtn.onClick.AddListener(PlayBtn_OnClick);
        }
        
        private void CloseBtn_OnClick()
        {
            
        }
        
        private void PlayBtn_OnClick()
        {
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
                _playLabelBtn.Id = _continueLocKey;
            }
            else
            {
                _playLabelBtn.Id = _playLocKey;
            }

            _panelAnimator.Play(_data.Instant);
            if (_data.Instant)
            {
                
            }
            else
            {
                
            }
        }
        
        public class Model
        {
         
        }
    }
    
    public class UIStartPanelData : UIScreenData
    {
        public GameProcessor GameProcessor { get; set; }
        public bool Instant { get; set; }
    }
}