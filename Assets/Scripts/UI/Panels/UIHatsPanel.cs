using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atom;
using Core.Gameplay;
using Core.Utils;
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
        [SerializeField] private UIHatsPanel_HatBlockItem _blockPrefab;
        [SerializeField] private GameObject _blockSeparatorPrefab;
        [SerializeField] private Button _buyBtn;
        [SerializeField] private Text _buyBtnPriceLabel;
        [SerializeField] private Button _equipBtn;
        [SerializeField] private Text _equipBtnLabel;
        [SerializeField] private GuidEx _equipTextKey;
        [SerializeField] private GuidEx _unequipTextKey;
        [SerializeField] private UIGameScreen_Coins _coins;
        
        private Model _model;
        private UIHatsPanelData _data;
        private HatItemModel _selectedItem;

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
            _selectedItem.SetUserActiveFilter(!_selectedItem.UserActive);
            
            var equipBtnTextKey = _selectedItem.UserActive ? _unequipTextKey : _equipTextKey;
            _equipBtnLabel.text = ApplicationController.Instance.LocalizationController.GetText(equipBtnTextKey);
            
            SetEquipAllLabel();
        }

        private void SetEquipAllLabel()
        {
            
        }

        public override void SetData(UIScreenData undefinedData)
        {
            base.SetData(undefinedData);

            _data = undefinedData as UIHatsPanelData;

            _model = new Model();
            _model.OnItemsUpdated += OnItemsUpdated;
            _model.OnBoughtButtonActiveChanged += OnBoughtButtonActiveChanged;
            _model.OnEquipStateChanged += EquipStateChanged;
            _model.OnEquipRestrictionsChanged += EquipRestrictionsChanged;
            
            _model.SetData(
                _data.Hats, 
                _data.UserActiveHatsFilter, 
                _data.HatsChanger, 
                _data.GameProcessor.ActiveGameRulesSettings,
                ApplicationController.Instance.SaveController.SaveProgress);
            OnItemsUpdated(_model.Items);
            
            _model.Select(_data.Selected);
            FocusOnBlock();
            
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
        
        private void OnItemsUpdated(IEnumerable<HatItemModel> items)
        {
            var oldViewBlocks = _container.content.GetComponents<UIHatsPanel_HatBlockItem>();
            foreach (var oldViewBlock in oldViewBlocks)
                Destroy(oldViewBlock.gameObject);
            
            _itemPrefab.gameObject.SetActive(false);
            _blockPrefab.gameObject.SetActive(false);
            _blockSeparatorPrefab.gameObject.SetActive(false);
            
            var hatsByGroupIndex = items
                .GroupBy(i => i.Hat.GroupIndex)
                .ToList();

            for (var groupI = 0; groupI < hatsByGroupIndex.Count; groupI++)
            {
                var hatsGroup = hatsByGroupIndex[groupI];
                var hatGroup = _data.GameProcessor.Scene.HatsLibrary.GetHatGroup(hatsGroup.Key);

                var blockView = Instantiate(_blockPrefab, _container.content);
                blockView.gameObject.SetActive(true);
                blockView.SetData(hatGroup.NameKey);

                foreach (var item in hatsGroup)
                {
                    var itemView = Instantiate(_itemPrefab, blockView.ContentRoot);
                    itemView.gameObject.SetActive(true);
                    itemView.SetModel(item, _data.GameProcessor);
                }

                if (groupI != hatsByGroupIndex.Count - 1)
                {   
                    var blockSeparator = Instantiate(_blockSeparatorPrefab, _container.content);
                    blockSeparator.gameObject.SetActive(true);
                }

                LayoutRebuilder.ForceRebuildLayoutImmediate(blockView.ContentRoot);
                LayoutRebuilder.ForceRebuildLayoutImmediate(_container.content);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(_container.content);
        }

        private void FocusOnBlock()
        {
            RectTransform focusOnHatBlock = null;

            var hatItems = GetComponentsInChildren<UIHatsPanel_HatItem>();
            foreach (var hatItem in hatItems)
            {
                if (hatItem.Model.Selected && hatItem.Model.Available && hatItem.Model.UserActive)
                {
                    var blockItem = hatItem.GetComponentInParent<UIHatsPanel_HatBlockItem>();
                    focusOnHatBlock = blockItem.Root;
                }
            }
            
            if (focusOnHatBlock != null)
            {
                var focusOnPosition = focusOnHatBlock.anchoredPosition.y + focusOnHatBlock.rect.height * 0.5f;
                var focusOnNormalPosition = focusOnPosition / (_container.content.rect.height - _container.viewport.rect.height);
                focusOnNormalPosition = 1.0f - Mathf.Clamp01(-focusOnNormalPosition);
                _container.normalizedPosition = new Vector2(0, focusOnNormalPosition);
            }
        }

        private void OnBoughtButtonActiveChanged(bool active, HatItemModel selected)
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
        
        private void EquipStateChanged(HatItemModel sender)
        {
            SetEquipLabel();
        }
        
        private void EquipRestrictionsChanged(int count, int maxActiveHats)
        {
            _equipAllLabel.text = $"{count}/{maxActiveHats}";
        }
        
        private void SetEquipLabel()
        {
            var equipBtnTextKey = _selectedItem.UserActive ? _unequipTextKey : _equipTextKey;
            _equipBtnLabel.text = ApplicationController.Instance.LocalizationController.GetText(equipBtnTextKey);
        }
        
        public class Model
        {
            public Action<IEnumerable<HatItemModel>> OnItemsUpdated;
            public Action<bool, HatItemModel> OnBoughtButtonActiveChanged;
            public Action<HatItemModel> OnEquipStateChanged;
            public Action<int, int> OnEquipRestrictionsChanged;

            private readonly List<HatItemModel> _items = new();
            private HatItemModel _selected;
            
            private IHatsChanger _changer;
            private GameRulesSettings _rulesSettings;
            private SaveProgress _saveProgress;
            
            public int MaxActiveHats => _rulesSettings.MaxActiveHats;
            public IEnumerable<HatItemModel> Items => _items;

            public int ActiveHatsCount
            {
                get
                {
                    var userActiveHatsFilter = _changer.GetUserActiveHatsFilter();

                    if (userActiveHatsFilter == null)
                        return 0;
                    
                    return _items
                        .Count(hat => hat.Available && userActiveHatsFilter.FirstOrDefault(hatName => hatName == hat.Id) != null);
                }
            }
            
            public void SetData(
                IReadOnlyList<Hat> hats,
                string[] userActiveFilter, 
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
                    var item = new HatItemModel(hat, this);
                    item.OnUserActiveFilterStateChanged += ItemOnUserActiveFilterStateChanged;

                    var userActive = false;
                    if (userActiveFilter != null)
                        userActive = Array.FindIndex(userActiveFilter, i => i == hat.Id) >= 0;

                    item.SetData(userActive);
                    _items.Add(item);
                }
            }

            private void ItemOnUserActiveFilterStateChanged(HatItemModel sender)
            {
                var activeFilter = new List<string>();
                for (var itemI = 0; itemI < _items.Count; itemI++)
                {
                    var item = _items[itemI];
                    if (item.UserActive)
                        activeFilter.Add(item.Hat.Id);
                }

                _changer.SetUserActiveHatsFilter(activeFilter.ToArray());
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
                    _selected.SetUserActiveFilter(true);
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
            
            internal void TrySelect(HatItemModel newSelected)
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
                    var userActiveHatsFilter = _changer.GetUserActiveHatsFilter();
                    var hatWithMinimumExtraPoints = _items
                        .OrderBy(i => i.ExtraPoints)
                        .FirstOrDefault(hat => hat.Available && userActiveHatsFilter.FirstOrDefault(hatName => hatName == hat.Id) != null);

                    if (hatWithMinimumExtraPoints != null)
                    {
                        hatWithMinimumExtraPoints.SetUserActiveFilter(false);
                    }
                }

                return true;
            }
        }
        private DependencyHolder<SoundsPlayer> _soundsPlayer;

        public void SetActiveHat(string hat)
        {
            var hatItems = _model.Items.FirstOrDefault(i => i.Hat.Id == hat);
            hatItems.SelectMe();
            hatItems.SetUserActiveFilter(true);
            
            _soundsPlayer.Value.Play(UICommonSounds.Click);
        }
    }
    
    public class UIHatsPanelData : UIScreenData
    {
        public GameProcessor GameProcessor;
        public Hat Selected;
        public string[] UserActiveHatsFilter;
        public IReadOnlyList<Hat> Hats;
        public IHatsChanger HatsChanger;
    }
}