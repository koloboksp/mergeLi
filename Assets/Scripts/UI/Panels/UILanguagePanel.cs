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
            var data = undefinedData as UILanguagePanelData;
            
            _model = new Model()
                .OnItemsUpdated(OnItemsUpdated)
                .OnItemsSelected(OnItemSelected);
            
            _model.SetData(data.Available, data.Selected, data.Changer);
        }

        private void OnItemSelected(UILanguagePanel_LanguageItem.Model sender)
        {
            LockInput(true);
            ApplicationController.Instance.UIPanelController.PopScreen(this);
            LockInput(false);
        }

        private void OnItemsUpdated(IEnumerable<UILanguagePanel_LanguageItem.Model> items)
        {
            var oldViews = _itemsContainer.content.GetComponents<UISkinPanel_SkinItem>();
            foreach (var oldView in oldViews)
                Destroy(oldView.gameObject);

            _itemPrefab.gameObject.SetActive(false);
            foreach (var item in items)
            {
                var itemView = Instantiate(_itemPrefab, _itemsContainer.content);
                itemView.gameObject.SetActive(true);
                itemView.SetModel(item);
            }
        }
        
        public class Model
        {
            private Action<IEnumerable<UILanguagePanel_LanguageItem.Model>> _onItemsUpdated;
            private Action<UILanguagePanel_LanguageItem.Model> _onItemSelected;
            
            private readonly List<UILanguagePanel_LanguageItem.Model> _items = new ();
            private ILanguageChanger _changer;
            public IEnumerable<UILanguagePanel_LanguageItem.Model> Items => _items;

            public void SetData(IEnumerable<UILanguagePanelLanguageData> languages, SystemLanguage selected, ILanguageChanger changer)
            {
                _changer = changer;
                
                _items.AddRange(languages
                    .Select(i => new UILanguagePanel_LanguageItem.Model(this)
                        .SetLanguage(i.Language)
                        .SetIcon(i.Icon)
                        .SetLabel(i.Label)));
                _onItemsUpdated?.Invoke(_items);

                foreach (var item in _items)
                    item.SetSelectedState(item.Language == selected);
            }

            public Model OnItemsUpdated(Action<IEnumerable<UILanguagePanel_LanguageItem.Model>> onItemsUpdated)
            {
                _onItemsUpdated = onItemsUpdated;
                return this;
            }
            
            public Model OnItemsSelected(Action<UILanguagePanel_LanguageItem.Model> onItemSelected)
            {
                _onItemSelected = onItemSelected;
                return this;
            }

            internal void TrySelect(UILanguagePanel_LanguageItem.Model newSelected)
            {
                _changer.SetLanguage(newSelected.Language);
                _onItemSelected?.Invoke(newSelected);
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