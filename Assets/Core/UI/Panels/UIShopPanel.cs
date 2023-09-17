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
        private CancellationTokenSource _cancellationTokenSource;

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
            var data = undefinedData as UIShopScreenData;

            _model = new Model()
                .OnItemsUpdated(OnItemsUpdated)
                .OnBoughtSomething(OnBoughtSomething)
                .OnLockInput(OnLockInput);
            
            _model.SetData(data.Market, data.PurchaseItems);
        }

        private void OnLockInput(bool state)
        {
            LockInput(state);
        }

        private void OnItemsUpdated(IEnumerable<UIShopPanel_PurchaseItem.Model> items)
        {
            var oldViews = _purchasesContainer.content.GetComponents<UISkinPanel_SkinItem>();
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
        
        private void OnBoughtSomething(bool success)
        {
            if (success)
            {
                StartCoroutine(PlayBoughtSuccessAnimation());
            }
            else
            {
                var panelData = new UIPurchaseFailedPanelData();
                ApplicationController.Instance.UIPanelController.PushPopupScreenAsync(
                    typeof(UIPurchaseFailedPanel), panelData, _cancellationTokenSource.Token);
            }
        }

        private IEnumerator PlayBoughtSuccessAnimation()
        {
            //stub
            yield return new WaitForSeconds(2.0f);
            ApplicationController.Instance.UIPanelController.PopScreen(this);
        }
        
        public class Model
        {
            private Action<IEnumerable<UIShopPanel_PurchaseItem.Model>> _onItemsUpdated;
            private Action<bool> _onBoughtSomething;
            private Action<bool> _onLockInput;

            private IMarket _market;
            private readonly List<UIShopPanel_PurchaseItem.Model> _items = new();
          
            public void SetData(IMarket market, IEnumerable<PurchaseItem> purchaseItems)
            {
                _market = market;
                _items.AddRange(purchaseItems
                    .Select(i => new UIShopPanel_PurchaseItem.Model(this)
                        .Init(i.PurchaseType == PurchaseType.Market ? i.ProductId : $"ShowAds{i.CurrencyAmount}", i.ProductId, i.PurchaseType)));
                _onItemsUpdated?.Invoke(_items);
            }
            
            public async Task<bool> Buy(UIShopPanel_PurchaseItem.Model model)
            {
                _onLockInput?.Invoke(true);
                
                var success = await _market.Buy(model.InAppId, model.PurchaseType);
                _onBoughtSomething?.Invoke(success);
                
                _onLockInput?.Invoke(false);
                return success;
            }
            
            public Model OnItemsUpdated(Action<IEnumerable<UIShopPanel_PurchaseItem.Model>> onItemsUpdated)
            {
                _onItemsUpdated = onItemsUpdated;
                _onItemsUpdated?.Invoke(_items);
                return this;
            }
            
            public Model OnBoughtSomething(Action<bool> onBoughtSomething)
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
        public IMarket Market;
        public IEnumerable<PurchaseItem> PurchaseItems;
    }
}