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
    public class UIGameScreen : UIScreen
    {
        [SerializeField] private CanvasGroup _buffsContainerRoot;
        [SerializeField] private ScrollRect _buffsContainer;
        [SerializeField] private UIScore _score;
        [SerializeField] private UICoins _coins;
        [SerializeField] private RectTransform _fieldContainer;

        private UIGameScreenData _data;
        private PointsGoal _currentPointsGoal = null;
        public UIGameScreenData Data => _data;
        
        public void Awake()
        {
            _coins.OnClick += Coins_OnClick;
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

            var fieldRoot = _data.GameProcessor.Scene.Field.View.Root;
            if (fieldRoot is RectTransform)
            {
                var fieldRootRect = fieldRoot as RectTransform;
                var containerRect = _fieldContainer.rect;
                var rect = fieldRootRect.rect;
                fieldRootRect.parent = _fieldContainer;
                //fieldRootRect.anchorMin = Vector2.zero;
                //fieldRootRect.anchorMax = Vector2.one;
                //fieldRootRect.offsetMin = Vector2.zero;
                //fieldRootRect.offsetMax = Vector2.zero;
                fieldRootRect.localPosition = Vector3.zero;
                fieldRootRect.localScale = new Vector3(containerRect.width/rect.width, containerRect.height/rect.height, 1);
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
        
        private void ShowSkinBtn_OnClick()
        {
            var skinScreenData = new UISkinScreen.UISkinScreenData();
            skinScreenData.SelectedSkin = _data.GameProcessor.Scene.ActiveSkin.Name;
            skinScreenData.Skins = _data.GameProcessor.Scene.Library.Containers.Select(i => i.Name);
            skinScreenData.SkinChanger = _data.GameProcessor.Scene;
            ApplicationController.Instance.UIScreenController.PushPopupScreen(typeof(UISkinScreen), skinScreenData);
        }
        
        private void Coins_OnClick()
        {
            ApplicationController.Instance.UIScreenController.PushPopupScreen(typeof(UIShopScreen),
                new UIShopScreenData()
                {
                    Market = _data.GameProcessor.Market,
                    PurchaseItems = new List<PurchaseItem>(_data.GameProcessor.Scene.PurchasesLibrary.Items)
                });
        }
    }

    public class UIGameScreenData : UIScreenData
    {
        public GameProcessor GameProcessor { get; set; }
    }
}