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
        [SerializeField] private Text _equipAllLabel;
        [SerializeField] private ScrollRect _container;
        [SerializeField] private UIHatsPanel_HatItem _itemPrefab;
        [SerializeField] private Button _buyBtn;
        [SerializeField] private Text _buyBtnPriceLabel;
        [SerializeField] private Button _equipBtn;
        [SerializeField] private Text _equipBtnLabel;
        [SerializeField] private GuidEx _equipTextKey;
        [SerializeField] private GuidEx _unequipTextKey;
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
                SetEquipAllLabel();
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
            _selectedItem.SetUserInactiveFilter(!_selectedItem.UserInactive);
            
            var equipBtnTextKey = _selectedItem.UserInactive ? _equipTextKey : _unequipTextKey;
            _equipBtnLabel.text = ApplicationController.Instance.LocalizationController.GetText(equipBtnTextKey);
            
            SetEquipAllLabel();
        }

        private void SetEquipAllLabel()
        {
            var maxActiveHats = _data.GameProcessor.ActiveGameRulesSettings.MaxActiveHats;
            var count = _data.Hats.Count(hat => hat.Available 
                                                && _data.UserInactiveHatsFilter != null 
                                                && _data.UserInactiveHatsFilter.FirstOrDefault(hatName => hatName == hat.Id) == null);
           
        }

        public override void SetData(UIScreenData undefinedData)
        {
            _data = undefinedData as UIHatsPanelData;

            _model = new Model();
            _model.OnItemsUpdated += OnItemsUpdated;
            _model.OnBoughtButtonActiveChanged += OnBoughtButtonActiveChanged;
            _model.OnEquipStateChanged += EquipStateChanged;
            _model.OnEquipRestrictionsChanged += EquipRestrictionsChanged;
            
            _model.SetData(
                _data.Hats, 
                _data.UserInactiveHatsFilter, 
                _data.HatsChanger, 
                _data.GameProcessor.ActiveGameRulesSettings,
                ApplicationController.Instance.SaveController.SaveProgress);
            _model.Select(_data.Selected);

            EquipRestrictionsChanged(_model.ActiveHatsCount, _model.MaxActiveHats);
            SetEquipAllLabel();
            
            ApplicationController.Instance.SaveController.SaveProgress.OnConsumeCurrency += SaveController_OnConsumeCurrency;
            _coins.MakeSingle();
            _coins.Set(_data.GameProcessor.CurrencyAmount);
            
           
        }

        protected override void InnerHide()
        {
            base.InnerHide();
            
            ApplicationController.Instance.SaveController.SaveProgress.OnConsumeCurrency -= SaveController_OnConsumeCurrency;
        }

        protected override void InnerActivate()
        {
            base.InnerActivate();
            
            _coins.MakeSingle();
            _coins.Set(_data.GameProcessor.CurrencyAmount);
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

            RectTransform focusOnHat = null;

            _itemPrefab.gameObject.SetActive(false);
            foreach (var item in items)
            {
                var itemView = Instantiate(_itemPrefab, _container.content);
                itemView.gameObject.SetActive(true);
                itemView.SetModel(item, _data.GameProcessor);

                if (item.Available && !item.UserInactive)
                {
                    focusOnHat = itemView.Root;
                }
            }
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(_container.content);
            
            if (focusOnHat != null)
            {
                var focusOnPosition = focusOnHat.anchoredPosition.y;
                var focusOnNormalPosition = focusOnPosition / (_container.content.rect.height - _container.viewport.rect.height);
                focusOnNormalPosition = 1.0f - Mathf.Clamp01(-focusOnNormalPosition);
                _container.normalizedPosition = new Vector2(0, focusOnNormalPosition);
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
                SetEquipLabel();
            }
        }
        
        private void EquipStateChanged(UIHatsPanel_HatItem.Model sender)
        {
            SetEquipLabel();
        }
        
        private void EquipRestrictionsChanged(int count, int maxActiveHats)
        {
            _equipAllLabel.text = $"{count}/{maxActiveHats}";
        }
        
        private void SetEquipLabel()
        {
            var equipBtnTextKey = _selectedItem.UserInactive ? _equipTextKey : _unequipTextKey;
            _equipBtnLabel.text = ApplicationController.Instance.LocalizationController.GetText(equipBtnTextKey);
        }
        
        public class Model
        {
            public Action<IEnumerable<UIHatsPanel_HatItem.Model>> OnItemsUpdated;
            public Action<bool, UIHatsPanel_HatItem.Model> OnBoughtButtonActiveChanged;
            public Action<UIHatsPanel_HatItem.Model> OnEquipStateChanged;
            public Action<int, int> OnEquipRestrictionsChanged;

            private readonly List<UIHatsPanel_HatItem.Model> _items = new();
            private UIHatsPanel_HatItem.Model _selected;
            
            private IHatsChanger _changer;
            private GameRulesSettings _rulesSettings;
            private SaveProgress _saveProgress;
            
            public int MaxActiveHats => _rulesSettings.MaxActiveHats;

            public int ActiveHatsCount
            {
                get
                {
                    var userInactiveHatsFilter = _changer.GetUserInactiveHatsFilter();

                    return _items
                        .Count(hat => hat.Available && userInactiveHatsFilter.FirstOrDefault(hatName => hatName == hat.Id) == null);
                }
            }
            
            public void SetData(
                IReadOnlyList<Hat> hats,
                string[] userInactiveFilter, 
                IHatsChanger changer,
                GameRulesSettings rulesSettings,
                SaveProgress saveProgress)
            {
                _saveProgress = saveProgress;
                _rulesSettings = rulesSettings;
                _changer = changer;

                for (var hatI = 0; hatI < hats.Count; hatI++)
                {
                    var hat = hats[hatI];
                    var item = new UIHatsPanel_HatItem.Model(hat, this);
                    item.OnUserInactiveFilterStateChanged += Item_OnUserInactiveFilterStateChanged;

                    var userInactive = false;
                    if (userInactiveFilter != null)
                        userInactive = Array.FindIndex(userInactiveFilter, i => i == hat.Id) >= 0;

                    item.SetData(userInactive);
                    _items.Add(item);
                }
                
                OnItemsUpdated?.Invoke(_items);
            }

            private void Item_OnUserInactiveFilterStateChanged(UIHatsPanel_HatItem.Model sender)
            {
                var inactiveFilter = new List<string>();
                for (var itemI = 0; itemI < _items.Count; itemI++)
                {
                    var item = _items[itemI];
                    if (item.UserInactive)
                        inactiveFilter.Add(item.Hat.Id);
                }

                _changer.SetUserInactiveHatsFilter(inactiveFilter.ToArray());
                OnEquipStateChanged?.Invoke(sender);
                UpdateEquipRestrictions();
            }
            
            private void UpdateEquipRestrictions()
            {
                OnEquipRestrictionsChanged?.Invoke(ActiveHatsCount, MaxActiveHats);
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
                    OnBoughtButtonActiveChanged?.Invoke(!_selected.Available, _selected);
                    BalanceActivateHats();
                    UpdateEquipRestrictions();
                    
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
                
                OnBoughtButtonActiveChanged?.Invoke(!_selected.Available, _selected);
            }

            internal bool BalanceActivateHats()
            {
                if (ActiveHatsCount >= MaxActiveHats)
                {
                    var userInactiveHatsFilter = _changer.GetUserInactiveHatsFilter();
                    var hatWithMinimumExtraPoints = _items
                        .OrderBy(i => i.ExtraPoints)
                        .FirstOrDefault(hat => hat.Available && userInactiveHatsFilter.FirstOrDefault(hatName => hatName == hat.Id) == null);

                    if (hatWithMinimumExtraPoints != null)
                    {
                        hatWithMinimumExtraPoints.SetUserInactiveFilter(true);
                    }
                }

                return true;
            }
        }
    }
    
    public class UIHatsPanelData : UIScreenData
    {
        public GameProcessor GameProcessor;
        public Hat Selected;
        public string[] UserInactiveHatsFilter;
        public IReadOnlyList<Hat> Hats;
        public IHatsChanger HatsChanger;
    }
}