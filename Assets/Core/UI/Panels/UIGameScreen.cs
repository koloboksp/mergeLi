using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        [SerializeField] private GameObject _lowSpaceWarning;

        private UIGameScreenData _data;
        private CancellationTokenSource _cancellationTokenSource;

        public UIGameScreenData Data => _data;
        
        public void Awake()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _coins.OnClick += Coins_OnClick;
            _showSettingsBtn.onClick.AddListener(ShowSettingsBtn_OnClick); 
        }
  
        private void OnDestroy()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }
        
        public override void SetData(UIScreenData data)
        {
            base.SetData(data);
            
            _data = data as UIGameScreenData;
            _data.GameProcessor.OnStepCompleted += OnStepCompleted;
            _data.GameProcessor.OnBeforeStepStarted += OnBeforeStepStarted;
            _data.GameProcessor.OnLowEmptySpaceChanged += OnLowEmptySpaceChanged;
            OnLowEmptySpaceChanged(false);
            _data.GameProcessor.OnScoreChanged += OnScoreChanged;
            OnScoreChanged(0);
            _data.GameProcessor.PlayerInfo.OnCoinsChanged += OnCoinsChanged;
            OnCoinsChanged();
            
            _data.GameProcessor.CastleSelector.OnCastleChanged += CastleSelector_OnCastleChanged;
            CastleSelector_OnCastleChanged();
            _data.GameProcessor.CastleSelector.OnSelectedPartChanged += CastleSelector_OnSelectedPartChanged;
            
            foreach (var buff in _data.GameProcessor.Buffs)
            {
                var control = buff.CreateControl();
                control.transform.SetParent(_buffsContainer.content, false);
                control.transform.localScale = Vector3.one;
            }
        }

        protected override void InnerHide()
        {
            _data.GameProcessor.OnStepCompleted -= OnStepCompleted;
            _data.GameProcessor.OnBeforeStepStarted -= OnBeforeStepStarted;
            _data.GameProcessor.OnLowEmptySpaceChanged -= OnLowEmptySpaceChanged;
            
            _data.GameProcessor.OnScoreChanged -= OnScoreChanged;
            _data.GameProcessor.PlayerInfo.OnCoinsChanged -= OnCoinsChanged;
            
            _data.GameProcessor.CastleSelector.OnCastleChanged -= CastleSelector_OnCastleChanged;
            _data.GameProcessor.CastleSelector.OnSelectedPartChanged -= CastleSelector_OnSelectedPartChanged;
            base.InnerHide();
        }

        private void CastleSelector_OnCastleChanged()
        {
            var marks = _data.GameProcessor.CastleSelector.ActiveCastle.Parts.Select(i => i.Cost);
            _score.SetScoreMarks(marks);
        }
        
        private void OnBeforeStepStarted(Step sender, StepExecutionType executionType)
        {
            _buffsContainerRoot.interactable = false;
        }

        private void OnStepCompleted(Step sender, StepExecutionType executionType)
        {
            _buffsContainerRoot.interactable = true;
        }

        private void OnLowEmptySpaceChanged(bool state)
        {
            _lowSpaceWarning.SetActive(state);
        }
        
        private void OnScoreChanged(int additionalPoints)
        {
            var currentPointsGoal = _data.GameProcessor.CastleSelector.ActiveCastle.GetPoints();
            var nextPointsGoal = _data.GameProcessor.CastleSelector.ActiveCastle.GetCost();
            
            _score.SetNextGoalScore(currentPointsGoal, nextPointsGoal);
            _score.SetSessionScore(_data.GameProcessor.Score, _data.GameProcessor.BestSessionScore);
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
            ApplicationController.Instance.UIPanelController.PushPopupScreenAsync(typeof(UISettingsPanel), skinScreenData, _cancellationTokenSource.Token);
        }
        
        private void Coins_OnClick()
        {
            ApplicationController.Instance.UIPanelController.PushPopupScreenAsync(
                typeof(UIShopPanel),
                new UIShopScreenData()
                {
                    Market = _data.GameProcessor.Market,
                    PurchaseItems = new List<PurchaseItem>(_data.GameProcessor.PurchasesLibrary.Items)
                },
                _cancellationTokenSource.Token);
        }
        
        private void CastleSelector_OnSelectedPartChanged()
        {
            var pointsGoal = _data.GameProcessor.CastleSelector.ActiveCastle.GetPoints();
            var costGoal = _data.GameProcessor.CastleSelector.ActiveCastle.GetCost(); 
            
            _score.SetNextGoalScore(pointsGoal, costGoal);
        }

        public void HideAllElements()
        {
            _buffsContainerRoot.gameObject.SetActive(false);
            _buffsContainer.gameObject.SetActive(false);
            _score.gameObject.SetActive(false);
            _coins.gameObject.SetActive(false);
            _showSettingsBtn.gameObject.SetActive(false);
            _lowSpaceWarning.gameObject.SetActive(false);
        }

        public void SetActiveElement(UIGameScreenElement element, bool active)
        {
            if(element == UIGameScreenElement.ProgressBar)
                _score.gameObject.SetActive(active);
        }
    }

    public class UIGameScreenData : UIScreenData
    {
        public GameProcessor GameProcessor { get; set; }
    }

    public enum UIGameScreenElement
    {
        ProgressBar,
    }
}