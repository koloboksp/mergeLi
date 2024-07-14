using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Atom;
using Core.Gameplay;
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

        [SerializeField] private Button _showPauseBtn;

        [SerializeField] private UIBubbleDialog _noSpaceWarning;
        [SerializeField] private GuidEx _noSpaceWarningTextKey;
        [SerializeField] private UIAnyGiftIndicator _anyGiftIndicator;

        private readonly List<UIBuff> _uiBuffs = new();
        private UIGameScreenData _data;

        public UIGameScreenData Data => _data;

        public void Awake()
        {
            _coins.OnClick += Coins_OnClick;
            _showPauseBtn.onClick.AddListener(ShowPauseBtn_OnClick);
            _anyGiftIndicator.OnClick += AnyGiftIndicatorOnClick;
        }

        public override string GetLayerName()
        {
            return "gameScreenLayer";
        }

        public override void SetData(UIScreenData undefinedData)
        {
            base.SetData(undefinedData);

            _data = undefinedData as UIGameScreenData;
            _data.GameProcessor.OnStepCompleted += OnStepCompleted;
            _data.GameProcessor.OnBeforeStepStarted += OnBeforeStepStarted;
            _data.GameProcessor.SessionProcessor.OnLowEmptySpaceChanged += OnLowEmptySpaceChanged;
            _data.GameProcessor.SessionProcessor.OnFreeSpaceIsOverChanged += OnFreeSpaceIsOverChanged;
            OnLowEmptySpaceChanged(false);
            
            ApplicationController.Instance.SaveController.SaveProgress.OnConsumeCurrency += SaveController_OnConsumeCurrency;
            _coins.Set(_data.GameProcessor.CurrencyAmount);
            
            _data.GameProcessor.CastleSelector.OnCastleChanged += CastleSelector_OnCastleChanged;
            CastleSelector_OnCastleChanged(_data.GameProcessor.CastleSelector.ActiveCastle);
            
            var score = _data.GameProcessor.SessionProcessor.GetScore();
            var bestScore = ApplicationController.Instance.SaveController.SaveProgress.BestSessionScore;
            _score.SetSession(0.1f, score, score, bestScore, true);
            
            foreach (var buff in _data.GameProcessor.Buffs)
            {
                var uiBuff = Instantiate(buff.ControlPrefab, _buffsContainer.content);
                uiBuff.transform.localScale = Vector3.one;
                uiBuff.SetModel(buff);
                _uiBuffs.Add(uiBuff);
            }
        }

        protected override void InnerActivate()
        {
            base.InnerActivate();
            
            _coins.MakeSingle();
            _coins.Set(_data.GameProcessor.CurrencyAmount);
            _anyGiftIndicator.Set(_data.GameProcessor.GiftsMarket);
            _data.GameProcessor.PauseGameProcess(false);
        }

        protected override void InnerHide()
        {
            _data.GameProcessor.PauseGameProcess(true);

            foreach (var uiBuff in _uiBuffs)
            {
                uiBuff.UnsubscribeModel();
                Destroy(uiBuff.gameObject);
            }
            _uiBuffs.Clear();
            
            _data.GameProcessor.OnStepCompleted -= OnStepCompleted;
            _data.GameProcessor.OnBeforeStepStarted -= OnBeforeStepStarted;
            _data.GameProcessor.SessionProcessor.OnLowEmptySpaceChanged -= OnLowEmptySpaceChanged;
            _data.GameProcessor.SessionProcessor.OnFreeSpaceIsOverChanged -= OnFreeSpaceIsOverChanged;
            ApplicationController.Instance.SaveController.SaveProgress.OnConsumeCurrency -= SaveController_OnConsumeCurrency;

            _data.GameProcessor.CastleSelector.OnCastleChanged -= CastleSelector_OnCastleChanged;
            var activeCastle = _data.GameProcessor.CastleSelector.ActiveCastle;
            if (activeCastle != null)
            {
                activeCastle.View.OnPartBornStart -= CastleView_OnPartBornStart;
                activeCastle.View.OnPartCompleteStart -= CastleView_OnPartCompleteStart;
                activeCastle.View.OnPartProgressStart -= CastleView_OnPartProgressStart;
            }

            base.InnerHide();
        }
        
        private void CastleSelector_OnCastleChanged(Castle previousCastle)
        {
            if (previousCastle != null)
            {
                previousCastle.View.OnPartBornStart -= CastleView_OnPartBornStart;
                previousCastle.View.OnPartCompleteStart -= CastleView_OnPartCompleteStart;
                previousCastle.View.OnPartProgressStart -= CastleView_OnPartProgressStart;
                previousCastle.OnPointsAdd += Castle_OnPointsChanged;
                previousCastle.OnPointsRefund -= Castle_OnPointsChanged;
            }

            var activeCastle = _data.GameProcessor.CastleSelector.ActiveCastle;
            if (activeCastle != null)
            {
                activeCastle.View.OnPartBornStart += CastleView_OnPartBornStart;
                activeCastle.View.OnPartCompleteStart += CastleView_OnPartCompleteStart;
                activeCastle.View.OnPartProgressStart += CastleView_OnPartProgressStart;
                activeCastle.OnPointsAdd += Castle_OnPointsChanged;
                activeCastle.OnPointsRefund += Castle_OnPointsChanged;
                
                var nextPointsGoal = activeCastle.GetLastPartCost();
                var currentPointsGoal = activeCastle.GetLastPartPoints();
                var score = _data.GameProcessor.SessionProcessor.GetScore();
                var bestScore = ApplicationController.Instance.SaveController.SaveProgress.BestSessionScore;

                _score.InstantSet(currentPointsGoal, nextPointsGoal);
                _score.InstantSetSession(score, bestScore);
            }
        }

        private void CastleView_OnPartProgressStart(bool instant, float duration, int oldPoints, int newPoints, int maxPoints)
        {
            if (instant)
            {
                _score.InstantSet(newPoints, maxPoints);
            }
            else
            {
                _score.Set(duration, oldPoints, newPoints, maxPoints);
            }
        }

        private void CastleView_OnPartBornStart(bool instant, float duration, int maxPoints)
        {
            if (instant)
            {
                _score.InstantSet(0, maxPoints);
            }
            else
            {
                _score.Set(duration, 0, 0, maxPoints);
            }
        }

        private void CastleView_OnPartCompleteStart(bool instant, float duration)
        {
        }

        private void Castle_OnPointsChanged(int dPoints)
        {
            var score = _data.GameProcessor.SessionProcessor.GetScore();
            var bestScore = ApplicationController.Instance.SaveController.SaveProgress.BestSessionScore;

            _score.SetSession(0.1f, score, score, bestScore, false);
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
            
        }
        
        private void OnFreeSpaceIsOverChanged(bool state, bool noAvailableSteps)
        {   
            var warningVisible = state && !noAvailableSteps;
            _noSpaceWarning.gameObject.SetActive(warningVisible);
            if (warningVisible)
                _ = _noSpaceWarning.ShowTextAsync(_noSpaceWarningTextKey, false, Application.exitCancellationToken);
        }
        
        private void SaveController_OnConsumeCurrency(int amount)
        {
            _coins.Add(-amount, false);
        }

        private void ShowPauseBtn_OnClick()
        {
            var panelData = new UIPausePanelData();
            panelData.GameProcessor = _data.GameProcessor;
            ApplicationController.Instance.UIPanelController.PushPopupScreenAsync<UIPausePanel>(
                panelData,
                Application.exitCancellationToken);
        }

        private void Coins_OnClick()
        {
            ApplicationController.Instance.UIPanelController.PushPopupScreenAsync<UIShopPanel>(
                new UIShopPanelData()
                {
                    GameProcessor = _data.GameProcessor,
                    Market = _data.GameProcessor.Market,
                    Items = UIShopPanel.FillShopItems(_data.GameProcessor),
                },
                Application.exitCancellationToken);
        }

        private void AnyGiftIndicatorOnClick()
        {
            ApplicationController.Instance.UIPanelController.PushPopupScreenAsync<UIShopPanel>(
                new UIShopPanelData()
                {
                    GameProcessor = _data.GameProcessor,
                    Market = _data.GameProcessor.Market,
                    Items = UIShopPanel.FillShopItems(_data.GameProcessor),
                },
                Application.exitCancellationToken);
        }

        public void HideAllElements()
        {
            _buffsContainerRoot.gameObject.SetActive(false);
            _score.gameObject.SetActive(false);
            _coins.gameObject.SetActive(false);
            _anyGiftIndicator.gameObject.SetActive(false);
            _showPauseBtn.gameObject.SetActive(false);
            _noSpaceWarning.gameObject.SetActive(false);
        }

        public void SetActiveElement(UIGameScreenElement element, bool active)
        {
            if (element == UIGameScreenElement.ProgressBar)
                _score.gameObject.SetActive(active);
            if (element == UIGameScreenElement.Coins)
            {
                _coins.gameObject.SetActive(active);
                _anyGiftIndicator.gameObject.SetActive(active);
            }
            if (element == UIGameScreenElement.Buffs)
                _buffsContainerRoot.gameObject.SetActive(active);
            if (element == UIGameScreenElement.Settings)
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