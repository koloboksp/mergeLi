using System;
using System.Collections.Generic;
using System.Linq;
using Core.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class UISkinPanel : UIPanel
    {
        [SerializeField] private Button _closeBtn;
        [SerializeField] private ScrollRect _skinsContainer;
        [SerializeField] private UISkinPanel_SkinItem _itemPrefab;
        
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

            var data = undefinedData as UISkinPanelData;
            
            _model = new Model()
                .OnItemsUpdated(OnItemsUpdated);
            
            _model.SetData(data.Skins, data.SelectedSkin, data.SkinChanger);
        }

        private void OnItemsUpdated(IEnumerable<UISkinPanel_SkinItem.Model> items)
        {
            var oldViews = _skinsContainer.content.GetComponents<UISkinPanel_SkinItem>();
            foreach (var oldView in oldViews)
                Destroy(oldView.gameObject);

            _itemPrefab.gameObject.SetActive(false);
            foreach (var item in items)
            {
                var itemView = Instantiate(_itemPrefab, _skinsContainer.content);
                itemView.gameObject.SetActive(true);
                itemView.SetModel(item);
            }
        }
        
        public class UISkinPanelData : UIScreenData
        {
            public string SelectedSkin;
            public IEnumerable<string> Skins;
            public ISkinChanger SkinChanger;
        }
        
        public class Model
        {
            private Action<IEnumerable<UISkinPanel_SkinItem.Model>> _onItemsUpdated;
            
            private readonly List<UISkinPanel_SkinItem.Model> _items = new List<UISkinPanel_SkinItem.Model>();
            private ISkinChanger _skinChanger;
            
            public void SetData(IEnumerable<string> skins, string selectedSkin, ISkinChanger skinChanger)
            {
                _skinChanger = skinChanger;
                
                _items.AddRange(skins
                    .Select(i => new UISkinPanel_SkinItem.Model(this)
                        .SetName(i)));
                _onItemsUpdated?.Invoke(_items);

                TrySelect(_items.Find(i => i.Name == selectedSkin));
            }

            public Model OnItemsUpdated(Action<IEnumerable<UISkinPanel_SkinItem.Model>> onItemsUpdated)
            {
                _onItemsUpdated = onItemsUpdated;
                _onItemsUpdated?.Invoke(_items);
                return this;
            }

            internal void TrySelect(UISkinPanel_SkinItem.Model newSelected)
            {
                foreach (var item in _items)
                    item.SetSelectedState(item == newSelected);
                
                _skinChanger.SetSkin(newSelected.Name);
            }
        }
    }
}