using System;
using System.Collections.Generic;
using System.Linq;
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
                .OnItemsUpdated(OnItemsUpdated);
            
            _model.SetData(data.Available, data.Selected, data.Changer);
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
        
        public class UILanguagePanelData : UIScreenData
        {
            public SystemLanguage Selected;
            public IEnumerable<SystemLanguage> Available;
            public ILanguageChanger Changer;
        }
        
        public class Model
        {
            private Action<IEnumerable<UILanguagePanel_LanguageItem.Model>> _onItemsUpdated;
            
            private readonly List<UILanguagePanel_LanguageItem.Model> _items = new ();
            private ILanguageChanger _changer;
            
            public void SetData(IEnumerable<SystemLanguage> languages, SystemLanguage selected, ILanguageChanger changer)
            {
                _changer = changer;
                
                _items.AddRange(languages
                    .Select(i => new UILanguagePanel_LanguageItem.Model(this)
                        .SetLanguage(i)
                        .SetIcon(ApplicationController.Instance.LocalizationController.GetIcon(i))));
                _onItemsUpdated?.Invoke(_items);

                TrySelect(_items.Find(i => i.Language == selected));
            }

            public Model OnItemsUpdated(Action<IEnumerable<UILanguagePanel_LanguageItem.Model>> onItemsUpdated)
            {
                _onItemsUpdated = onItemsUpdated;
                _onItemsUpdated?.Invoke(_items);
                return this;
            }

            internal void TrySelect(UILanguagePanel_LanguageItem.Model newSelected)
            {
                foreach (var item in _items)
                    item.SetSelectedState(item == newSelected);
                
                _changer.SetLanguage(newSelected.Language);
            }
        }
    }
}