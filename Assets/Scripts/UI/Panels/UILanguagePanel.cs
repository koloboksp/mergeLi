using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public interface ILanguageChanger
    {
        public void SetLanguage(SystemLanguage language);
    }
    public class UILanguagePanel : UIPanel
    {
        [SerializeField] private Button _closeBtn;
        [SerializeField] private ScrollRect _itemsContainer;
        [SerializeField] private UILanguagePanel_LanguageItem _itemPrefab;
        [SerializeField] private List<UILanguagePanel_LanguageItem> _items = new();

        private Model _model;

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
            base.SetData(undefinedData);

            var data = undefinedData as UILanguagePanelData;

            _model = new Model();
            _model.OnItemsUpdated += OnItemsUpdated;
            _model.OnItemSelected += OnItemSelected;
            _model.SetData(data.Available, data.Selected, data.Changer);
            OnItemsUpdated(_model.Items);
            FocusOnSelected();
        }

        private void OnItemSelected(LanguageItemModel sender)
        {
            LockInput(true);
            ApplicationController.Instance.UIPanelController.PopScreen(this);
            LockInput(false);
        }

        private void OnItemsUpdated(IEnumerable<LanguageItemModel> itemModels)
        {
            foreach (var item in _items)
            {
                Destroy(item.gameObject);
            }
            _items.Clear();

            _itemPrefab.gameObject.SetActive(false);
            foreach (var itemModel in itemModels)
            {
                var item = Instantiate(_itemPrefab, _itemsContainer.content);
                item.gameObject.SetActive(true);
                item.SetModel(itemModel);
                _items.Add(item);
                
                LayoutRebuilder.ForceRebuildLayoutImmediate(_itemsContainer.content);
            }
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(_itemsContainer.content);
        }
        
        private void FocusOnSelected()
        {
            RectTransform focusedRect = null;

            var focusedItem = _items.Find(i => i.Model == _model.Selected);
            if (focusedItem != null)
            {
                focusedRect = focusedItem.Root;
            }
            
            if (focusedRect != null)
            {
                var focusOnPosition = focusedRect.anchoredPosition.y;
                var focusOnNormalPosition = focusOnPosition / (_itemsContainer.content.rect.height - _itemsContainer.viewport.rect.height);
                focusOnNormalPosition = 1.0f - Mathf.Clamp01(-focusOnNormalPosition);
                _itemsContainer.normalizedPosition = new Vector2(0, focusOnNormalPosition);
            }
        }
        
        public class Model
        {
            public Action<IEnumerable<LanguageItemModel>> OnItemsUpdated;
            public Action<LanguageItemModel> OnItemSelected;
            
            private readonly List<LanguageItemModel> _items = new ();
            private LanguageItemModel _selected;
            private ILanguageChanger _changer;
            
            public IEnumerable<LanguageItemModel> Items => _items;
            public LanguageItemModel Selected => _selected;
            
            public void SetData(IEnumerable<UILanguagePanelLanguageData> languages, SystemLanguage selected, ILanguageChanger changer)
            {
                _changer = changer;
                
                _items.AddRange(languages
                    .Select(i => new LanguageItemModel(this)
                        .SetLanguage(i.Language)
                        .SetIcon(i.Icon)
                        .SetLabel(i.Label)));

                foreach (var item in _items)
                {
                    if (item.Language == selected)
                    {
                        _selected = item;
                        item.SetSelectedState(true);
                    }
                    else
                    {
                        item.SetSelectedState(false);
                    }
                }
            }
            
            internal void TrySelect(LanguageItemModel newSelected)
            {
                _selected = newSelected;
                _changer.SetLanguage(_selected.Language);
                OnItemSelected?.Invoke(_selected);
            }
        }
    }
    
    public class UILanguagePanelData : UIScreenData
    {
        public SystemLanguage Selected;
        public IEnumerable<UILanguagePanelLanguageData> Available;
        public ILanguageChanger Changer;
    }
    
    public class UILanguagePanelLanguageData : UIScreenData
    {
        public SystemLanguage Language;
        public Sprite Icon;
        public string Label;
    }
}