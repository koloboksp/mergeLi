using System;
using System.Collections.Generic;
using System.Linq;
using Skins;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class UIHatsPanel : UIPanel
    {
        [SerializeField] private Button _closeBtn;
        [SerializeField] private ScrollRect _container;
        [SerializeField] private UIHatsPanel_HatItem _itemPrefab;
        [SerializeField] private Button _buyBtn;
        [SerializeField] private Text _alreadyHaveLabel;

        private Model _model;
        private UIHatsPanelData _data;
        
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
            _data = undefinedData as UIHatsPanelData;
            
            _model = new Model()
                .OnItemsUpdated(OnItemsUpdated)
                .OnBoughtButtonActiveChanged(OnBoughtButtonActiveChanged);
            
            _model.SetData(_data.Hats, _data.SelectedHat, _data.HatsChanger);
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
            if (active)
            {
                _buyBtn.gameObject.SetActive(true);
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
            private IHatsChanger _changer;
            
            public void SetData(IEnumerable<Hat> hats, Hat selectedHat, IHatsChanger changer)
            {
                _changer = changer;
                
                _items.AddRange(hats
                    .Select(i => new UIHatsPanel_HatItem.Model(i, this)));
                _onItemsUpdated?.Invoke(_items);

                var initialSelected = _items.Find(i => i.Hat == selectedHat);
                if (initialSelected == null)
                    initialSelected = _items.FirstOrDefault();
                
                if(initialSelected != null)
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

            internal void TrySelect(UIHatsPanel_HatItem.Model newSelected)
            {
                foreach (var item in _items)
                    item.SetSelectedState(item == newSelected);

                if (newSelected.Available)
                    _changer.SetHat(newSelected.Id);
                
                _onBoughtButtonActiveChanged?.Invoke(!newSelected.Available, newSelected);
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