using System;
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
        [SerializeField] private Button _showSkinBtn;
        [SerializeField] private ScrollRect _buffsContainer;
        [SerializeField] private UIScore _score;
        [SerializeField] private UICoins _coins;

        private UIGameScreenData _data;

       
        public void Awake()
        {
            _showSkinBtn.onClick.AddListener(ShowSkinBtn_OnClick);
            
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
        }

        private void OnStepExecute(Step sender)
        {
            //_undoBtn.interactable = false;
        }

        private void OnStepCompleted(Step sender)
        {
            //_undoBtn.interactable = true;
        }

        private void OnScoreChanged()
        {
            _score.SetScore(_data.GameProcessor.Score);
        }

        private void OnCoinsChanged()
        {
            _coins.SetCoins(_data.GameProcessor.PlayerInfo.GetAvailableCoins());
        }
        
        private void UndoBtn_OnClick()
        {
            _data.GameProcessor.Undo();
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