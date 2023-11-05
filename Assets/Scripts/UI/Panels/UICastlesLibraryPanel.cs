using System;
using System.Collections.Generic;
using System.Linq;
using Core.Goals;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class UICastlesLibraryPanel : UIPanel
    {
        [SerializeField] private Button _closeBtn;
        [SerializeField] private ScrollRect _container;
        
        private Model _model;
        private GameProcessor _gameProcessor;
        
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
            var data = undefinedData as UICastleLibraryPanelData;
            _gameProcessor = data.GameProcessor;
            
            _model = new Model()
                .OnItemsUpdated(OnItemsUpdated);
            
            _model.SetData(data.Castles, data.Selected);
        }

        private void OnItemsUpdated(IEnumerable<Castle> itemsPrefabs)
        {
            var oldViews = _container.content.GetComponents<Castle>();
            foreach (var oldView in oldViews)
                Destroy(oldView.gameObject);
            
            foreach (var itemsPrefab in itemsPrefabs)
            {
                var item = Instantiate(itemsPrefab, _container.content);
                item.gameObject.name = itemsPrefab.Id;
                item.View.Root.anchorMin = Vector2.zero;
                item.View.Root.anchorMax = Vector2.one;
                item.View.Root.offsetMin = Vector2.zero;
                item.View.Root.offsetMax = Vector2.zero;
                item.View.Root.localScale = Vector3.one;
        
                item.Init(_gameProcessor);
            }
        }
        
        public class UICastleLibraryPanelData : UIScreenData
        {
            public string Selected;
            public IEnumerable<Castle> Castles;
            public GameProcessor GameProcessor;
        }
        
        public class Model
        {
            private Action<IEnumerable<Castle>> _onItemsUpdated;
            
            private readonly List<Castle> _items = new ();
            
            public void SetData(IEnumerable<Castle> castles, string selectedCastle)
            {
                _items.AddRange(castles);
                _onItemsUpdated?.Invoke(_items);

                TrySelect(_items.Find(i => i.Id == selectedCastle));
            }

            public Model OnItemsUpdated(Action<IEnumerable<Castle>> onItemsUpdated)
            {
                _onItemsUpdated = onItemsUpdated;
                _onItemsUpdated?.Invoke(_items);
                return this;
            }

            internal void TrySelect(Castle newSelected)
            {
                
            }
        }
    }
}