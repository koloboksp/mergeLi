using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Atom;
using Core.Ads;
using Core.Gameplay;
using Core.Market;
using Core.Steps.CustomOperations;
using Core.Utils;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Math = System.Math;
using Object = UnityEngine.Object;

namespace Core
{
    public class UIShopPanel : UIPanel
    {
        [SerializeField] private Button _closeBtn;
        [SerializeField] private UIGameScreen_Coins _coins;
        [SerializeField] private ScrollRect _purchasesContainer;
        [SerializeField] private UIShopPanel_Item _marketItemPrefab;
        [SerializeField] private UIShopPanel_Item _adsItemPrefab;
        [SerializeField] private UIShopPanel_Item _giftItemPrefab;

        private Model _model;
        private UIShopPanelData _data;
        
        private readonly List<UIShopPanel_Item> _items = new();
       // private readonly List<(UIOverCastleCompletePanel i, Transform parent, bool activeSelf)> _overUIElements = new();
        
        private void Awake()
        {
            _closeBtn.onClick.AddListener(CloseBtn_OnClick);
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
            
            ApplicationController.Instance.SaveController.SaveProgress.OnConsumeCurrency += SaveController_OnConsumeCurrency;
            _coins.MakeSingle();
            _coins.Set(_data.GameProcessor.CurrencyAmount);
            
        }

       
        
        protected override void InnerActivate()
        {
            base.InnerActivate();
           
            _coins.MakeSingle();
            _coins.Set(_data.GameProcessor.CurrencyAmount);
           // var overUIElements = FindObjectsOfType<UIOverCastleCompletePanel>(true)
           //     .Select(i => (i, i.transform.parent, i.gameObject.activeSelf))
           //     .ToList();
           // 
           // foreach (var overUIElementTuple in overUIElements)
           // {
           //     if (_overUIElements.FindIndex(i=> i.i == overUIElementTuple.i) < 0)
           //     {
           //         overUIElementTuple.i.transform.SetParent(transform, true);
           //         overUIElementTuple.i.gameObject.SetActive(overUIElementTuple.activeSelf);
           //         _overUIElements.Add(overUIElementTuple);
           //     }
           // }
        }

        protected override void InnerHide()
        {
            ApplicationController.Instance.SaveController.SaveProgress.OnConsumeCurrency -= SaveController_OnConsumeCurrency;
            
            base.InnerHide();
        }
        
        private void SaveController_OnConsumeCurrency(int amount)
        {
            _coins.Add(-amount, false);
        }

        private void OnLockInput(bool state)
        {
            LockInput(state);
        }

        private void OnItemsUpdated(IEnumerable<UIShopPanel_ItemModel> items)
        {
            foreach (var oldView in _items)
                Destroy(oldView.gameObject);
            _items.Clear();

            _marketItemPrefab.gameObject.SetActive(false);
            _adsItemPrefab.gameObject.SetActive(false);
            _giftItemPrefab.gameObject.SetActive(false);
            
            foreach (var item in items)
            {
                var itemView = item.Item switch
                {
                    ShopPanelMarketItem => Instantiate(_marketItemPrefab, _purchasesContainer.content),
                    ShopPanelAdsItem => Instantiate(_adsItemPrefab, _purchasesContainer.content),
                    ShopPanelGiftItem => Instantiate(_giftItemPrefab, _purchasesContainer.content),
                    _ => null
                };
                itemView.gameObject.SetActive(true);
                itemView.SetModel(item);
                
                _items.Add(itemView);
            }
        }
        
        private async Task OnBoughtSomethingAsync(UIShopPanel_ItemModel sender, bool success, int coinsAmount)
        {
            if (success)
            {
                await PlayBoughtSuccessAnimation(sender, coinsAmount, Application.exitCancellationToken);
            }
            else
            {
                var panelData = new UIPurchaseFailedPanelData();
                ApplicationController.Instance.UIPanelController.PushPopupScreenAsync<UIPurchaseFailedPanel>(
                    panelData, 
                    Application.exitCancellationToken);
            }
        }

        private async Task PlayBoughtSuccessAnimation(UIShopPanel_ItemModel sender, int coinsAmount, CancellationToken cancellationToken)
        {
            //stub
            LockInput(true);
            var item = _items.Find(i => i.Model == sender);
            
            await _data.GameProcessor.GiveCoinsEffect.Show(coinsAmount, item.Root, cancellationToken);
            await AsyncExtensions.WaitForSecondsAsync(2.0f, Application.exitCancellationToken);
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
        
        public static IEnumerable<IShopPanelItem> FillShopItems(GameProcessor gameProcessor)
        {
            var items = new List<IShopPanelItem>();

            items.AddRange(gameProcessor.AdsLibrary.Items
                .Select(i => new ShopPanelAdsItem()
                    .Init(i.name, i.CurrencyAmount, gameProcessor.AdsViewer)));
            items.AddRange(gameProcessor.PurchasesLibrary.Items
                .Select(i => new ShopPanelMarketItem()
                    .Init(i.name, i.ProductId, i.CurrencyAmount, gameProcessor.Market)
                    .SetIcon(i.Icon)));
            items.AddRange(gameProcessor.GiftsMarket.Gifts
                .Select(i => new ShopPanelGiftItem()
                    .Init(i.Id, i)));

            return items;
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
        IShopPanelItem SetIcon(Sprite icon);
    }

    public class ShopPanelMarketItem : IShopPanelItem
    {
        private UIShopPanel.Model _owner;
        private string _name;
        private string _productId;
        private string _backgroundName;
        private int _currencyAmount;
        private Sprite _icon;

        private IMarket _market;
        
        public string Name => _name;
        public int CurrencyAmount => _currencyAmount;
        
        public string ProductId => _productId;
        public Sprite Icon => _icon;

        public IMarket Market => _market;
        
        public IShopPanelItem Init(string name, string inAppId, int currencyAmount, IMarket market)
        {
            _name = name;
            _productId = inAppId;
            _currencyAmount = currencyAmount;
            _market = market;
               
            return this;
        }
        
        public IShopPanelItem SetIcon(Sprite icon)
        {
            _icon = icon;
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
        public IShopPanelItem SetIcon(Sprite icon)
        {
            throw new NotImplementedException();
        }

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
        private long _restTimer;
        private GiftModel _gift;
        
        public int CurrencyAmount => _gift.CurrencyAmount;
        public IShopPanelItem SetIcon(Sprite icon)
        {
            throw new NotImplementedException();
        }

        public string Name => _name;
        public GiftModel Gift => _gift;
        
        public IShopPanelItem Init(string name, GiftModel gift)
        {
            _name = name;
            _gift = gift;
            
            return this;
        }
    }
}