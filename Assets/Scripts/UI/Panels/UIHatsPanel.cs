using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class UIHatsPanel : UIPanel
    {
        [SerializeField] private Button _closeBtn;
        [SerializeField] private ScrollRect _container;
        [SerializeField] private UIHatsPanel_HatItem _itemPrefab;
        
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
            var data = undefinedData as UIHatsPanelData;
            
            _model = new Model()
                .OnItemsUpdated(OnItemsUpdated);
            
            _model.SetData(data.Skins, data.SelectedSkin, data.HatsChanger);
        }

        private void OnItemsUpdated(IEnumerable<UIHatsPanel_HatItem.Model> items)
        {
            var oldViews = _container.content.GetComponents<UISkinPanel_SkinItem>();
            foreach (var oldView in oldViews)
                Destroy(oldView.gameObject);

            _itemPrefab.gameObject.SetActive(false);
            foreach (var item in items)
            {
                var itemView = Instantiate(_itemPrefab, _container.content);
                itemView.gameObject.SetActive(true);
                itemView.SetModel(item);
            }
        }
        
       
        
        public class Model
        {
            private Action<IEnumerable<UIHatsPanel_HatItem.Model>> _onItemsUpdated;
            
            private readonly List<UIHatsPanel_HatItem.Model> _items = new List<UIHatsPanel_HatItem.Model>();
            private IHatsChanger _changer;
            
            public void SetData(IEnumerable<string> hats, string selectedSkin, IHatsChanger changer)
            {
                _changer = changer;
                
                _items.AddRange(hats
                    .Select(i => new UIHatsPanel_HatItem.Model(this)
                        .SetName(i)));
                _onItemsUpdated?.Invoke(_items);

                TrySelect(_items.Find(i => i.Name == selectedSkin));
            }

            public Model OnItemsUpdated(Action<IEnumerable<UIHatsPanel_HatItem.Model>> onItemsUpdated)
            {
                _onItemsUpdated = onItemsUpdated;
                _onItemsUpdated?.Invoke(_items);
                return this;
            }

            internal void TrySelect(UIHatsPanel_HatItem.Model newSelected)
            {
                foreach (var item in _items)
                    item.SetSelectedState(item == newSelected);
                
                _changer.SetHat(newSelected.Name);
            }
        }
    }
    
    public class UIHatsPanelData : UIScreenData
    {
        public string SelectedSkin;
        public IEnumerable<string> Skins;
        public IHatsChanger HatsChanger;
    }
}