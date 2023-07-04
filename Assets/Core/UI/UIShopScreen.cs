using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Steps.CustomOperations;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class UIShopScreen : UIScreen
    {
        [SerializeField] private Button _closeBtn;
        [SerializeField] private ScrollRect _purchasesContainer;
        [SerializeField] private UIShopScreen_PurchaseItem _itemPrefab;

        private Model _model;

        private void Awake()
        {
            _closeBtn.onClick.AddListener(CloseBtn_OnClick);
        }
        
        private void CloseBtn_OnClick()
        {
            ApplicationController.Instance.UIScreenController.PopScreen(this);
        }
        
        public override void SetData(UIScreenData undefinedData)
        {
            var data = undefinedData as UIShopScreenData;

            _model = new Model()
                .OnItemsUpdated(OnItemsUpdated)
                .OnBoughtSomething(OnBoughtSomething);
            
            _model.SetData(data.Market, data.PurchaseItems);
        }
        
        private void OnItemsUpdated(IEnumerable<UIShopScreen_PurchaseItem.Model> items)
        {
            var oldViews = _purchasesContainer.content.GetComponents<UISkinScreen_SkinItem>();
            foreach (var oldView in oldViews)
                Destroy(oldView.gameObject);

            _itemPrefab.gameObject.SetActive(false);
            foreach (var item in items)
            {
                var itemView = Instantiate(_itemPrefab, _purchasesContainer.content);
                itemView.gameObject.SetActive(true);
                itemView.SetModel(item);
            }
        }
        
        private void OnBoughtSomething()
        {
            ApplicationController.Instance.UIScreenController.PopScreen(this);
        }
        
        public class Model
        {
            private Action<IEnumerable<UIShopScreen_PurchaseItem.Model>> _onItemsUpdated;
            private Action _onBoughtSomething;

            private IMarket _market;
            private readonly List<UIShopScreen_PurchaseItem.Model> _items = new();
          
            public void SetData(IMarket market, IEnumerable<PurchaseItem> purchaseItems)
            {
                _market = market;
                _items.AddRange(purchaseItems
                    .Select(i => new UIShopScreen_PurchaseItem.Model(this)
                        .Init(i.PurchaseType == PurchaseType.Market ? i.InAppId : $"ShowAds{i.CurrencyAmount}", i.InAppId, i.PurchaseType)));
                _onItemsUpdated?.Invoke(_items);
            }
            
            public async Task<bool> Buy(UIShopScreen_PurchaseItem.Model model)
            {
                var success = await _market.Buy(model.InAppId, model.PurchaseType);
                if(success)
                    _onBoughtSomething?.Invoke();
                return success;
            }
            
            public Model OnItemsUpdated(Action<IEnumerable<UIShopScreen_PurchaseItem.Model>> onItemsUpdated)
            {
                _onItemsUpdated = onItemsUpdated;
                _onItemsUpdated?.Invoke(_items);
                return this;
            }
            
            public Model OnBoughtSomething(Action onBoughtSomething)
            {
                _onBoughtSomething = onBoughtSomething;
                return this;
            }
        }
    }
    
    public class UIShopScreenData : UIScreenData
    {
        public IMarket Market;
        public IEnumerable<PurchaseItem> PurchaseItems;
    }
}