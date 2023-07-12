using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Steps;
using Core.Steps.CustomOperations;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class UIGameScreen : UIPanel
    {
        [SerializeField] private CanvasGroup _buffsContainerRoot;
        [SerializeField] private ScrollRect _buffsContainer;
        [SerializeField] private UIGameScreen_Score _score;
        [SerializeField] private UIGameScreen_Coins _coins;
        [SerializeField] private RectTransform _fieldContainer;
        [SerializeField] private Button _showSettingsBtn;

        private UIGameScreenData _data;
        private PointsGoal _currentPointsGoal = null;
        public UIGameScreenData Data => _data;
        
        public void Awake()
        {
            _coins.OnClick += Coins_OnClick;
            _showSettingsBtn.onClick.AddListener(ShowSettingsBtn_OnClick); 
        }

        public override void SetData(UIScreenData data)
        {
            base.SetData(data);
            
            _data = data as UIGameScreenData;
            _data.GameProcessor.OnStepCompleted += OnStepCompleted;
            _data.GameProcessor.OnStepExecute += OnStepExecute;
            _data.GameProcessor.OnScoreChanged += OnScoreChanged;
            OnScoreChanged();
            _data.GameProcessor.PlayerInfo.OnCoinsChanged += OnCoinsChanged;
            OnCoinsChanged();
            
            foreach (var buff in _data.GameProcessor.Buffs)
            {
                var control = buff.CreateControl();
                control.transform.SetParent(_buffsContainer.content);
            }
        }

        private void OnStepExecute(Step sender)
        {
            _buffsContainerRoot.interactable = false;
        }

        private void OnStepCompleted(Step sender)
        {
            _buffsContainerRoot.interactable = true;
        }

        
        private void OnScoreChanged()
        {
            var nextPointsGoal = _data.GameProcessor.GoalsLibrary.PointsGoals.GetNextGoal(_data.GameProcessor.Score);
            
            _score.SetScore(_data.GameProcessor.Score, nextPointsGoal.Threshold);
            if (_currentPointsGoal != nextPointsGoal)
            {
                if (_currentPointsGoal != null)
                    StartCoroutine(ShowGoalReachAnimation());
                
                _currentPointsGoal = nextPointsGoal;
            }
        }

        private IEnumerator ShowGoalReachAnimation()
        {
            yield return null;
        }

        private void OnCoinsChanged()
        {
            _coins.SetCoins(_data.GameProcessor.PlayerInfo.GetAvailableCoins());
        }
        
        private void ShowSettingsBtn_OnClick()
        {
            var skinScreenData = new UISettingsPanel.UISettingsPanelData();
            skinScreenData.GameProcessor = _data.GameProcessor;
            ApplicationController.Instance.UIPanelController.PushPopupScreen(typeof(UISettingsPanel), skinScreenData);
        }
        
        private void Coins_OnClick()
        {
            ApplicationController.Instance.UIPanelController.PushPopupScreen(typeof(UIShopPanel),
                new UIShopScreenData()
                {
                    Market = _data.GameProcessor.Market,
                    PurchaseItems = new List<PurchaseItem>(_data.GameProcessor.PurchasesLibrary.Items)
                });
        }
    }

    public class UIGameScreenData : UIScreenData
    {
        public GameProcessor GameProcessor { get; set; }
    }
}