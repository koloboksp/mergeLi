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

        [FormerlySerializedAs("_showSettingsBtn")] [SerializeField]
        private Button _showPauseBtn;

        [SerializeField] private GameObject _lowSpaceWarning;
        [FormerlySerializedAs("_anyGift")] [SerializeField] private UIAnyGiftIndicator _anyGiftIndicator;

        private List<UIBuff> _uiBuffs = new();
        private UIGameScreenData _data;

        public UIGameScreenData Data => _data;

        public void Awake()
        {
            _coins.OnClick += Coins_OnClick;
            _showPauseBtn.onClick.AddListener(ShowPauseBtn_OnClick);
            _anyGiftIndicator.OnClick += AnyGiftIndicatorOnClick;
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
            _data.GameProcessor.OnConsumeCurrency += GameProcessor_OnConsumeCurrency;
            OnConsumeCurrency(-_data.GameProcessor.CurrencyAmount, true);

            _data.GameProcessor.CastleSelector.OnCastleChanged += CastleSelector_OnCastleChanged;
            CastleSelector_OnCastleChanged(_data.GameProcessor.CastleSelector.ActiveCastle);

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
            _data.GameProcessor.OnLowEmptySpaceChanged -= OnLowEmptySpaceChanged;

            _data.GameProcessor.OnScoreChanged -= OnScoreChanged;
            _data.GameProcessor.OnConsumeCurrency -= GameProcessor_OnConsumeCurrency;

            _data.GameProcessor.CastleSelector.OnCastleChanged -= CastleSelector_OnCastleChanged;
            var activeCastle = _data.GameProcessor.CastleSelector.ActiveCastle;
            if (activeCastle != null)
            {
                activeCastle.View.OnPartBornStart -= View_OnPartBornStart;
                activeCastle.View.OnPartCompleteStart -= View_OnPartCompleteStart;
                activeCastle.View.OnPartProgressStart -= View_OnPartProgressStart;
            }

            base.InnerHide();
        }
        
        private void CastleSelector_OnCastleChanged(Castle previousCastle)
        {
            if (previousCastle != null)
            {
                previousCastle.View.OnPartBornStart -= View_OnPartBornStart;
                previousCastle.View.OnPartCompleteStart -= View_OnPartCompleteStart;
                previousCastle.View.OnPartProgressStart -= View_OnPartProgressStart;
            }

            var activeCastle = _data.GameProcessor.CastleSelector.ActiveCastle;
            if (activeCastle != null)
            {
                activeCastle.View.OnPartBornStart += View_OnPartBornStart;
                activeCastle.View.OnPartCompleteStart += View_OnPartCompleteStart;
                activeCastle.View.OnPartProgressStart += View_OnPartProgressStart;

                var nextPointsGoal = activeCastle.GetLastPartCost();
                var currentPointsGoal = activeCastle.GetLastPartPoints();
                _score.InstantSet(currentPointsGoal, nextPointsGoal);
            }
        }

        private void View_OnPartProgressStart(bool instant, float duration, int oldPoints, int newPoints, int maxPoints)
        {
            if (instant)
                _score.InstantSet(newPoints, maxPoints);
            else
                _score.Set(duration, oldPoints, newPoints, maxPoints);
        }

        private void View_OnPartBornStart(bool instant, float duration)
        {
        }

        private void View_OnPartCompleteStart(bool instant, float duration)
        {
            if (instant)
                _score.InstantComplete();
            else
                _score.Complete(duration);
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
            //_score.SetSessionScore(_data.GameProcessor.Score, _data.GameProcessor.BestSessionScore);
        }

        private IEnumerator ShowGoalReachAnimation()
        {
            yield return null;
        }

        private void GameProcessor_OnConsumeCurrency(int amount)
        {
            OnConsumeCurrency(amount, false);
        }

        private void OnConsumeCurrency(int amount, bool force)
        {
            _coins.Add(-amount, force);
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
            _showPauseBtn.gameObject.SetActive(false);
            _lowSpaceWarning.gameObject.SetActive(false);
        }

        public void SetActiveElement(UIGameScreenElement element, bool active)
        {
            if (element == UIGameScreenElement.ProgressBar)
                _score.gameObject.SetActive(active);
            if (element == UIGameScreenElement.Coins)
                _coins.gameObject.SetActive(active);
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