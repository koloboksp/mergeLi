using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Steps.CustomOperations;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Core
{
    public class UIShopPanel : UIPanel
    {
        [SerializeField] private Button _closeBtn;
        [SerializeField] private ScrollRect _purchasesContainer;
        [SerializeField] private UIShopPanel_Item _marketItemPrefab;
        [SerializeField] private UIShopPanel_Item _adsItemPrefab;
        [SerializeField] private UIShopPanel_Item _giftItemPrefab;

        private Model _model;
        private UIShopPanelData _data;
        private CancellationTokenSource _cancellationTokenSource;

        private readonly List<UIShopPanel_Item> _items = new();
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
            _data = undefinedData as UIShopPanelData;
            _model = new Model()
                .OnItemsUpdated(OnItemsUpdated)
                .OnBoughtSomething(OnBoughtSomethingAsync)
                .OnLockInput(OnLockInput);
            
            _model.SetData(_data.Items);
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

        private void OnItemsUpdated(IEnumerable<UIShopPanel_ItemModel> items)
        {
            foreach (var oldView in _items)
                Destroy(oldView.gameObject);

            _marketItemPrefab.gameObject.SetActive(false);
            _adsItemPrefab.gameObject.SetActive(false);
            _giftItemPrefab.gameObject.SetActive(false);
            
            foreach (var item in items)
            {
                UIShopPanel_Item itemView = item.Item switch
                {
                    ShopPanelMarketItem => Instantiate(_marketItemPrefab, _purchasesContainer.content),
                    ShopPanelAdsItem => Instantiate(_adsItemPrefab, _purchasesContainer.content),
                    ShopPanelGiftItem => Instantiate(_giftItemPrefab, _purchasesContainer.content),
                    _ => null
                };
                _items.Add(itemView);
                itemView.gameObject.SetActive(true);
                itemView.SetModel(item);
            }
        }
        
        private async Task OnBoughtSomethingAsync(UIShopPanel_ItemModel sender, bool success, int coinsAmount)
        {
            if (success)
            {
                await PlayBoughtSuccessAnimation(sender, coinsAmount, _cancellationTokenSource.Token);
            }
            else
            {
                var panelData = new UIPurchaseFailedPanelData();
                ApplicationController.Instance.UIPanelController.PushPopupScreenAsync<UIPurchaseFailedPanel>(
                    panelData, _cancellationTokenSource.Token);
            }
        }

        private async Task PlayBoughtSuccessAnimation(UIShopPanel_ItemModel sender, int coinsAmount, CancellationToken cancellationToken)
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
            private Action<IEnumerable<UIShopPanel_ItemModel>> _onItemsUpdated;
            private Func<UIShopPanel_ItemModel, bool, int, Task> _onBoughtSomething;
            private Action<bool> _onLockInput;

            private readonly List<UIShopPanel_ItemModel> _itemModels = new();
          
            public void SetData(IEnumerable<IShopPanelItem> purchaseItems)
            {
                _itemModels.AddRange(purchaseItems
                    .Select(i => new UIShopPanel_ItemModel(this)
                        .Init(i)));
                _onItemsUpdated?.Invoke(_itemModels);
            }
            
            public void ChangeLockInputState(bool state)
            {
                _onLockInput?.Invoke(state);
            }
            
            public async Task SomethingBoughtAsync(UIShopPanel_ItemModel sender, bool success, int amount)
            {
                if(_onBoughtSomething != null)
                    await _onBoughtSomething.Invoke(sender, success, amount);
            }
            
            public Model OnItemsUpdated(Action<IEnumerable<UIShopPanel_ItemModel>> onItemsUpdated)
            {
                _onItemsUpdated = onItemsUpdated;
                _onItemsUpdated?.Invoke(_itemModels);
                return this;
            }
            
            public Model OnBoughtSomething(Func<UIShopPanel_ItemModel, bool, int, Task> onBoughtSomething)
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
    
    public class UIShopPanelData : UIScreenData
    {
        public GameProcessor GameProcessor;
        public IMarket Market;
        public IEnumerable<IShopPanelItem> Items;
    }

    public interface IShopPanelItem
    {
        string Name { get; }
        int CurrencyAmount { get; }
    }

    public class ShopPanelMarketItem : IShopPanelItem
    {
        private UIShopPanel.Model _owner;
        private string _name;
        private string _productId;
        private string _backgroundName;
        private int _currencyAmount;
        
        private IMarket _market;
        
        public string Name => _name;
        public int CurrencyAmount => _currencyAmount;
        public string ProductId => _productId;

        public IMarket Market => _market;
        
        public IShopPanelItem Init(string name, string inAppId, int currencyAmount, IMarket market)
        {
            _name = name;
            _productId = inAppId;
            _currencyAmount = currencyAmount;
            _market = market;
               
            return this;
        }
    }
    
    public class ShopPanelAdsItem : IShopPanelItem
    {
        private UIShopPanel.Model _owner;
        private string _name;
        private int _currencyAmount;
        private IAdsViewer _adsViewer;

        public string Name => _name;
        public int CurrencyAmount => _currencyAmount;
        public IAdsViewer AdsViewer => _adsViewer;

        public IShopPanelItem Init(string name, int currencyAmount, IAdsViewer adsViewer)
        {
            _name = name;
            _currencyAmount = currencyAmount;
            _adsViewer = adsViewer;
            
            return this;
        }
    }
    
    public class ShopPanelGiftItem : IShopPanelItem
    {
        private UIShopPanel.Model _owner;
        private string _name;
        private int _currencyAmount;
        
        public int CurrencyAmount => _currencyAmount;
        public string Name => _name;

        public IShopPanelItem Init(string name, int currencyAmount)
        {
            _name = name;
            _currencyAmount = currencyAmount;
                
            return this;
        }
    }
}