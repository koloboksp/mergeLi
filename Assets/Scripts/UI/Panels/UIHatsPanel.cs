using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atom;
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
        [SerializeField] private Button _equipBtn;
        [SerializeField] private Text _equipBtnLabel;
        [SerializeField] private GuidEx _equipTextKey;
        [SerializeField] private GuidEx _unequipTextKey;

      //  [SerializeField] private Text _alreadyHaveLabel;
        [SerializeField] private UIGameScreen_Coins _coins;
        
        private Model _model;
        private UIHatsPanelData _data;
        private UIHatsPanel_HatItem.Model _selectedItem;
        
        private void Awake()
        {
            _closeBtn.onClick.AddListener(CloseBtn_OnClick);
            _buyBtn.onClick.AddListener(BuyBtn_OnClick);
            _equipBtn.onClick.AddListener(EquipBtn_OnClick);
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

        private void EquipBtn_OnClick()
        {
            _selectedItem.SetUserInactiveFilter(!_selectedItem.UserInactiveFilter);
            
            var equipBtnTextKey = _selectedItem.UserInactiveFilter ? _equipTextKey : _unequipTextKey;
            _equipBtnLabel.text = ApplicationController.Instance.LocalizationController.GetText(equipBtnTextKey);
        }
        
        public override void SetData(UIScreenData undefinedData)
        {
            _data = undefinedData as UIHatsPanelData;
            
            _model = new Model()
                .OnItemsUpdated(OnItemsUpdated)
                .OnBoughtButtonActiveChanged(OnBoughtButtonActiveChanged);
            
            _model.SetData(
                _data.Hats, 
                _data.UserInactiveHatsFilter, 
                _data.HatsChanger, 
                ApplicationController.Instance.SaveController.SaveProgress);
            _model.Select(_data.Selected);
            
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
                _equipBtn.gameObject.SetActive(false);
            }
            else
            {
                _buyBtn.gameObject.SetActive(false);
                _equipBtn.gameObject.SetActive(true);
                var equipBtnTextKey = _selectedItem.UserInactiveFilter ? _equipTextKey : _unequipTextKey;
                _equipBtnLabel.text = ApplicationController.Instance.LocalizationController.GetText(equipBtnTextKey);
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
                IReadOnlyList<Hat> hats,
                int[] userInactiveFilter, 
                IHatsChanger changer,
                SaveProgress saveProgress)
            {
                _saveProgress = saveProgress;
                _changer = changer;

                for (var hatI = 0; hatI < hats.Count; hatI++)
                {
                    var hat = hats[hatI];
                    var item = new UIHatsPanel_HatItem.Model(hat, this)
                        .SetUserInactiveFilter(Array.FindIndex(userInactiveFilter, i => i == hatI) >= 0);
                    item.OnUserInactiveFilterStateChanged += Item_OnUserInactiveFilterStateChanged;
                    _items.Add(item);
                }
                
                _onItemsUpdated?.Invoke(_items);
            }

            private void Item_OnUserInactiveFilterStateChanged()
            {
                var inactiveFilter = new List<int>();
                for (var itemI = 0; itemI < _items.Count; itemI++)
                {
                    var item = _items[itemI];
                    if (item.UserInactiveFilter)
                        inactiveFilter.Add(itemI);
                }

                _changer.SetUserInactiveHatsFilter(inactiveFilter.ToArray());
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

            public void Select(Hat hat)
            {
                var item = _items.Find(i => i.Hat == hat);
                TrySelect(item);
            }
            
            internal void TrySelect(UIHatsPanel_HatItem.Model newSelected)
            {
                _selected = newSelected;
                
                foreach (var item in _items)
                    item.SetSelectedState(item == _selected);
                
                _onBoughtButtonActiveChanged?.Invoke(!_selected.Available, _selected);
            }
        }
    }
    
    public class UIHatsPanelData : UIScreenData
    {
        public GameProcessor GameProcessor;
        public Hat Selected;
        public int[] UserInactiveHatsFilter;
        public IReadOnlyList<Hat> Hats;
        public IHatsChanger HatsChanger;
    }
}