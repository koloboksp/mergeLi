using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Core.Goals;
using Core.Steps;
using Core.Steps.CustomOperations;
using UnityEngine;
using UnityEngine.Serialization;
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
        [FormerlySerializedAs("_showSettingsBtn")] [SerializeField] private Button _showPauseBtn;
        [SerializeField] private GameObject _lowSpaceWarning;

        private UIGameScreenData _data;
        private CancellationTokenSource _cancellationTokenSource;

        public UIGameScreenData Data => _data;
        
        public void Awake()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _coins.OnClick += Coins_OnClick;
            _showPauseBtn.onClick.AddListener(ShowPauseBtn_OnClick); 
        }
  
        private void OnDestroy()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }
        
        public override void SetData(UIScreenData undefinedData)
        {
            base.SetData(undefinedData);
            
            _data = undefinedData as UIGameScreenData;
            _data.GameProcessor.OnStepCompleted += OnStepCompleted;
            _data.GameProcessor.OnBeforeStepStarted += OnBeforeStepStarted;
            _data.GameProcessor.OnLowEmptySpaceChanged += OnLowEmptySpaceChanged;
            OnLowEmptySpaceChanged(false);
            _data.GameProcessor.OnScoreChanged += OnScoreChanged;
            OnScoreChanged(0);
            _data.GameProcessor.OnConsumeCurrency += (amount) => OnConsumeCurrency(amount, false);
            OnConsumeCurrency(-_data.GameProcessor.CurrencyAmount, true);
            
            _data.GameProcessor.CastleSelector.OnCastleChanged += CastleSelector_OnCastleChanged;
            CastleSelector_OnCastleChanged(null);
            
            foreach (var buff in _data.GameProcessor.Buffs)
            {
                var control = buff.CreateControl();
                control.transform.SetParent(_buffsContainer.content, false);
                control.transform.localScale = Vector3.one;
            }
        }

        protected override void InnerActivate()
        {
            base.InnerActivate();
            
            _data.GameProcessor.PauseGameProcess(false);
        }

        protected override void InnerHide()
        {
            _data.GameProcessor.PauseGameProcess(true);
            
            _data.GameProcessor.OnStepCompleted -= OnStepCompleted;
            _data.GameProcessor.OnBeforeStepStarted -= OnBeforeStepStarted;
            _data.GameProcessor.OnLowEmptySpaceChanged -= OnLowEmptySpaceChanged;
            
            _data.GameProcessor.OnScoreChanged -= OnScoreChanged;
         //   _data.GameProcessor.PlayerInfo.OnCoinsChanged -= OnCoinsChanged;
            
            _data.GameProcessor.CastleSelector.OnCastleChanged -= CastleSelector_OnCastleChanged;
            
            base.InnerHide();
        }

        private void CastleSelector_OnCastleChanged(Castle previousCastle)
        {
            if (previousCastle != null)
            {
                previousCastle.OnProgressChanged -= ActiveCastle_OnProgressChanged;
            }
            
            var activeCastle = _data.GameProcessor.CastleSelector.ActiveCastle;
            if (activeCastle != null)
            {
                activeCastle.OnProgressChanged += ActiveCastle_OnProgressChanged;
            }
            
            var marks = activeCastle.Parts.Select(i => i.Cost);
            _score.SetScoreMarks(marks);
        }

        private void ActiveCastle_OnProgressChanged()
        {
            var activeCastle = _data.GameProcessor.CastleSelector.ActiveCastle;
            var nextPointsGoal = activeCastle.GetCost();
            var currentPointsGoal = activeCastle.GetPoints();
            _score.SetNextGoalScore(currentPointsGoal, nextPointsGoal);
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
            
          
            _score.SetSessionScore(_data.GameProcessor.Score, _data.GameProcessor.BestSessionScore);
        }

        private IEnumerator ShowGoalReachAnimation()
        {
            yield return null;
        }

        private void OnConsumeCurrency(int amount, bool force)
        {
            _coins.Add(-amount, force);
        }
        
        private void ShowPauseBtn_OnClick()
        {
            var panelData = new UIPausePanelData();
            panelData.GameProcessor = _data.GameProcessor;
            ApplicationController.Instance.UIPanelController.PushPopupScreenAsync<UIPausePanel>(panelData, _cancellationTokenSource.Token);
        }
        
        private void Coins_OnClick()
        {
            ApplicationController.Instance.UIPanelController.PushPopupScreenAsync<UIShopPanel>(
                new UIShopPanelData()
                {
                    GameProcessor = _data.GameProcessor,
                    Market = _data.GameProcessor.Market,
                    Items = FillShopItems(_data.GameProcessor),
                },
                _cancellationTokenSource.Token);
        }

        public static IEnumerable<IShopPanelItem> FillShopItems(GameProcessor gameProcessor)
        {
            var items = new List<IShopPanelItem>();

            items.AddRange(gameProcessor.AdsLibrary.Items
                .Select(i => new ShopPanelAdsItem()
                    .Init(i.name, i.CurrencyAmount, gameProcessor.AdsViewer)));
            items.AddRange(gameProcessor.PurchasesLibrary.Items
                .Select(i => new ShopPanelMarketItem()
                    .Init(i.name, i.ProductId, i.CurrencyAmount, gameProcessor.Market)));
            items.AddRange(gameProcessor.GiftsLibrary.Items
                .Select(i => new ShopPanelGiftItem()
                    .Init(i.name, i.CurrencyAmount)));

            return items;
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
            _score.gameObject.SetActive(false);
            _coins.gameObject.SetActive(false);
            _showPauseBtn.gameObject.SetActive(false);
            _lowSpaceWarning.gameObject.SetActive(false);
        }

        public void SetActiveElement(UIGameScreenElement element, bool active)
        {
            if(element == UIGameScreenElement.ProgressBar)
                _score.gameObject.SetActive(active);
            if(element == UIGameScreenElement.Coins)
                _coins.gameObject.SetActive(active);
            if(element == UIGameScreenElement.Buffs)
                _buffsContainerRoot.gameObject.SetActive(active);
            if(element == UIGameScreenElement.Settings)
                _showPauseBtn.gameObject.SetActive(active);
        }
    }

    public class UIGameScreenData : UIScreenData
    {
        public GameProcessor GameProcessor { get; set; }
    }

    public enum UIGameScreenElement
    {
        ProgressBar,
        Coins,
        Buffs,
        Settings,
    }
}