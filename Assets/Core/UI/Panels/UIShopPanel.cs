using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Steps.CustomOperations;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class UIShopPanel : UIPanel
    {
        [SerializeField] private Button _closeBtn;
        [SerializeField] private ScrollRect _purchasesContainer;
        [SerializeField] private UIShopPanel_PurchaseItem _itemPrefab;

        private Model _model;
        private UIShopScreenData _data;
        private CancellationTokenSource _cancellationTokenSource;

        private readonly List<UIShopPanel_PurchaseItem> _items = new();
        private readonly List<(UIOverCastleCompletePanel i, Transform parent, bool activeSelf)> _overUIElements = new();
        
        private void Awake()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _closeBtn.onClick.AddListener(CloseBtn_OnClick);
        }

        private void OnDestroy()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        private void CloseBtn_OnClick()
        {
            ApplicationController.Instance.UIPanelController.PopScreen(this);
        }
        
        public override void SetData(UIScreenData undefinedData)
        {
            _data = undefinedData as UIShopScreenData;
            _model = new Model()
                .OnItemsUpdated(OnItemsUpdated)
                .OnBoughtSomething(OnBoughtSomething)
                .OnLockInput(OnLockInput);
            
            _model.SetData(_data.Market, _data.PurchaseItems);
        }

       
        protected override void InnerActivate()
        {
            base.InnerActivate();
           
            var overUIElements = FindObjectsOfType<UIOverCastleCompletePanel>(true)
                .Select(i => (i, i.transform.parent, i.gameObject.activeSelf))
                .ToList();
            foreach (var overUIElementTuple in overUIElements)
            {
                if (_overUIElements.FindIndex(i=> i.i == overUIElementTuple.i) < 0)
                {
                    overUIElementTuple.i.transform.SetParent(transform, true);
                    overUIElementTuple.i.gameObject.SetActive(overUIElementTuple.activeSelf);
                    _overUIElements.Add(overUIElementTuple);
                }
            }
        }

        protected override void InnerHide()
        {
            foreach (var overUIElementTuple in _overUIElements)
                overUIElementTuple.i.transform.SetParent(overUIElementTuple.parent, true);

            base.InnerHide();
        }

        private void OnLockInput(bool state)
        {
            LockInput(state);
        }

        private void OnItemsUpdated(IEnumerable<UIShopPanel_PurchaseItemModel> items)
        {
            foreach (var oldView in _items)
                Destroy(oldView.gameObject);

            _itemPrefab.gameObject.SetActive(false);
            foreach (var item in items)
            {
                var itemView = Instantiate(_itemPrefab, _purchasesContainer.content);
                _items.Add(itemView);
                itemView.gameObject.SetActive(true);
                itemView.SetModel(item);
            }
        }
        
        private async void OnBoughtSomething(UIShopPanel_PurchaseItemModel sender, bool success, int coinsAmount, CancellationToken cancellationToken)
        {
            if (success)
            {
                await PlayBoughtSuccessAnimation(sender, coinsAmount, cancellationToken);
            }
            else
            {
                var panelData = new UIPurchaseFailedPanelData();
                ApplicationController.Instance.UIPanelController.PushPopupScreenAsync(
                    typeof(UIPurchaseFailedPanel), panelData, _cancellationTokenSource.Token);
            }
        }

        private async Task PlayBoughtSuccessAnimation(UIShopPanel_PurchaseItemModel sender, int coinsAmount, CancellationToken cancellationToken)
        {
            //stub
            LockInput(true);
            var item = _items.Find(i => i.Model == sender);
            
            await _data.GameProcessor.GiveCoinsEffect.Show(coinsAmount, item.Root, cancellationToken);
            await AsyncExtensions.WaitForSecondsAsync(2.0f, _cancellationTokenSource.Token);
            LockInput(false);
            ApplicationController.Instance.UIPanelController.PopScreen(this);
        }
        
        public class Model
        {
            private Action<IEnumerable<UIShopPanel_PurchaseItemModel>> _onItemsUpdated;
            private Action<UIShopPanel_PurchaseItemModel, bool, int, CancellationToken> _onBoughtSomething;
            private Action<bool> _onLockInput;

            private IMarket _market;
            private readonly List<UIShopPanel_PurchaseItemModel> _items = new();
          
            public void SetData(IMarket market, IEnumerable<PurchaseItem> purchaseItems)
            {
                _market = market;
                _items.AddRange(purchaseItems
                    .Select(i => new UIShopPanel_PurchaseItemModel(this)
                        .Init(i.PurchaseType == PurchaseType.Market ? i.ProductId : $"ShowAds{i.CurrencyAmount}", i.ProductId, i.CurrencyAmount, i.PurchaseType)
                        .SetBackgroundName(i.BackgroundName)));
                _onItemsUpdated?.Invoke(_items);
            }
            
            public async Task<bool> Buy(UIShopPanel_PurchaseItemModel model, CancellationToken cancellationToken)
            {
                _onLockInput?.Invoke(true);
                var result = await _market.Buy(model.InAppId, model.PurchaseType, cancellationToken);
                _onLockInput?.Invoke(false);
                
                _onBoughtSomething?.Invoke(model, result.success, result.amount, cancellationToken);
                
                return result.success;
            }
            
            public Model OnItemsUpdated(Action<IEnumerable<UIShopPanel_PurchaseItemModel>> onItemsUpdated)
            {
                _onItemsUpdated = onItemsUpdated;
                _onItemsUpdated?.Invoke(_items);
                return this;
            }
            
            public Model OnBoughtSomething(Action<UIShopPanel_PurchaseItemModel, bool, int, CancellationToken> onBoughtSomething)
            {
                _onBoughtSomething = onBoughtSomething;
                return this;
            }
            
            public Model OnLockInput(Action<bool> onLockInput)
            {
                _onLockInput = onLockInput;
                return this;
            }
        }
    }
    
    public class UIShopScreenData : UIScreenData
    {
        public GameProcessor GameProcessor;
        public IMarket Market;
        public IEnumerable<PurchaseItem> PurchaseItems;
    }
}