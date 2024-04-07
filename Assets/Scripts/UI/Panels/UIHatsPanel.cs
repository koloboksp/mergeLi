using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Save;
using Skins;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Core
{
    public class UIHatsPanel : UIPanel
    {
        [SerializeField] private Button _closeBtn;
        [SerializeField] private ScrollRect _container;
        [SerializeField] private UIHatsPanel_HatItem _itemPrefab;
        [SerializeField] private Button _buyBtn;
        [SerializeField] private Text _buyBtnPriceLabel;
        [SerializeField] private Text _alreadyHaveLabel;
        [SerializeField] private UIGameScreen_Coins _coins;
        
        private Model _model;
        private UIHatsPanelData _data;
        private UIHatsPanel_HatItem.Model _selectedItem;
        
        private void Awake()
        {
            _closeBtn.onClick.AddListener(CloseBtn_OnClick);
            _buyBtn.onClick.AddListener(BuyBtn_OnClick);
            _coins.OnClick += Coins_OnClick;
        }

        private void CloseBtn_OnClick()
        {
            ApplicationController.Instance.UIPanelController.PopScreen(this);
        }

        private async void BuyBtn_OnClick()
        {
            if (_model.CanBuySelectedItem())
            {
                var buyResult = await _model.BuySelectedItem();
                if (buyResult)
                {
                    
                }
            }
            else
            {
                await ApplicationController.Instance.UIPanelController.PushPopupScreenAsync<UIShopPanel>(
                    new UIShopPanelData()
                    {
                        GameProcessor = _data.GameProcessor,
                        Market = _data.GameProcessor.Market,
                        Items = UIShopPanel.FillShopItems(_data.GameProcessor),
                    },
                    Application.exitCancellationToken);
            }
        }
        
        public override void SetData(UIScreenData undefinedData)
        {
            _data = undefinedData as UIHatsPanelData;
            
            _model = new Model()
                .OnItemsUpdated(OnItemsUpdated)
                .OnBoughtButtonActiveChanged(OnBoughtButtonActiveChanged);
            
            _model.SetData(
                _data.Hats, 
                _data.SelectedHat, 
                _data.HatsChanger, 
                ApplicationController.Instance.SaveController.SaveProgress);
            
            ApplicationController.Instance.SaveController.SaveProgress.OnConsumeCurrency += SaveController_OnConsumeCurrency;
            OnConsumeCurrency(-_data.GameProcessor.CurrencyAmount, true);
        }

        protected override void InnerHide()
        {
            base.InnerHide();
            
            ApplicationController.Instance.SaveController.SaveProgress.OnConsumeCurrency -= SaveController_OnConsumeCurrency;
        }

        private void SaveController_OnConsumeCurrency(int amount)
        {
            OnConsumeCurrency(amount, false);
        }

        private void OnConsumeCurrency(int amount, bool force)
        {
            _coins.Add(-amount, force);
        }
        
        private async void Coins_OnClick()
        {
            await ApplicationController.Instance.UIPanelController.PushPopupScreenAsync<UIShopPanel>(
                new UIShopPanelData()
                {
                    GameProcessor = _data.GameProcessor,
                    Market = _data.GameProcessor.Market,
                    Items = UIShopPanel.FillShopItems(_data.GameProcessor),
                },
                Application.exitCancellationToken);
        }
        
        private void OnItemsUpdated(IEnumerable<UIHatsPanel_HatItem.Model> items)
        {
            var oldViews = _container.content.GetComponents<UIHatsPanel_HatItem>();
            foreach (var oldView in oldViews)
                Destroy(oldView.gameObject);

            _itemPrefab.gameObject.SetActive(false);
            foreach (var item in items)
            {
                var itemView = Instantiate(_itemPrefab, _container.content);
                itemView.gameObject.SetActive(true);
                itemView.SetModel(item, _data.GameProcessor);
            }
        }

        private void OnBoughtButtonActiveChanged(bool active, UIHatsPanel_HatItem.Model selected)
        {
            _selectedItem = selected;
            
            if (active)
            {
                _buyBtn.gameObject.SetActive(true);
                _buyBtnPriceLabel.text = _selectedItem.Cost.ToString();
                _alreadyHaveLabel.gameObject.SetActive(false);
            }
            else
            {
                _buyBtn.gameObject.SetActive(false);
                _alreadyHaveLabel.gameObject.SetActive(true);
            }
        }

        public class Model
        {
            private Action<IEnumerable<UIHatsPanel_HatItem.Model>> _onItemsUpdated;
            private Action<bool, UIHatsPanel_HatItem.Model> _onBoughtButtonActiveChanged;

            private readonly List<UIHatsPanel_HatItem.Model> _items = new List<UIHatsPanel_HatItem.Model>();
            private UIHatsPanel_HatItem.Model _selected;
            
            private IHatsChanger _changer;
            private SaveProgress _saveProgress;
            
            public void SetData(
                IEnumerable<Hat> hats,
                Hat selectedHat, 
                IHatsChanger changer,
                SaveProgress saveProgress)
            {
                _saveProgress = saveProgress;
                _changer = changer;
                
                _items.AddRange(hats
                    .Select(i => new UIHatsPanel_HatItem.Model(i, this)));
                _onItemsUpdated?.Invoke(_items);

                var initialSelected = _items.Find(i => i.Hat == selectedHat);
                if (initialSelected == null)
                    initialSelected = _items.FirstOrDefault();

                if (initialSelected != null)
                    TrySelect(initialSelected);
            }

            public Model OnItemsUpdated(Action<IEnumerable<UIHatsPanel_HatItem.Model>> onItemsUpdated)
            {
                _onItemsUpdated = onItemsUpdated;
                return this;
            }
            
            public Model OnBoughtButtonActiveChanged(Action<bool, UIHatsPanel_HatItem.Model> onChanged)
            {
                _onBoughtButtonActiveChanged = onChanged;
                return this;
            }

            public bool CanBuySelectedItem()
            {
                return _selected.Cost < _saveProgress.GetAvailableCoins();
            }

            public async Task<bool> BuySelectedItem()
            {
                if (_selected.Cost < _saveProgress.GetAvailableCoins())
                {
                    await _selected.Buy();
                    _onBoughtButtonActiveChanged?.Invoke(!_selected.Available, _selected);
                    
                    return true;
                }
                
                return false;
            }
            
            internal void TrySelect(UIHatsPanel_HatItem.Model newSelected)
            {
                _selected = newSelected;
                
                foreach (var item in _items)
                    item.SetSelectedState(item == _selected);

                if (_selected.Available)
                    _changer.SetHat(_selected.Id);
                
                _onBoughtButtonActiveChanged?.Invoke(!_selected.Available, _selected);
            }
        }
    }
    
    public class UIHatsPanelData : UIScreenData
    {
        public GameProcessor GameProcessor;
        public Hat SelectedHat;
        public IEnumerable<Hat> Hats;
        public IHatsChanger HatsChanger;
    }
}