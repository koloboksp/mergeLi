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
        [SerializeField] private UICastlesLibraryPanel_CastleLabel _castleLabelPrefab;

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

            _castleLabelPrefab.gameObject.SetActive(false);
            
            var contentHeight = itemsPrefabs.Sum(i => i.Root.sizeDelta.y + _castleLabelPrefab.Root.sizeDelta.y);
            _container.content.ForceUpdateRectTransforms();
            _container.content.sizeDelta = new Vector2(_container.content.sizeDelta.x, contentHeight);
            
            foreach (var itemsPrefab in itemsPrefabs)
            {
                var castleLabel = Instantiate(_castleLabelPrefab, _container.content);
                castleLabel.gameObject.SetActive(true);
                castleLabel.NameKey = itemsPrefab.NameKey;
                
                var castle = Instantiate(itemsPrefab, _container.content);
                castle.gameObject.name = itemsPrefab.Id;
                castle.SetData(_gameProcessor);
                if (castle.Root.sizeDelta.x > _container.content.sizeDelta.x)
                {
                    var scaleFactor = _container.content.sizeDelta.x / castle.Root.sizeDelta.x;
                    castle.Root.localScale = new Vector3(scaleFactor, scaleFactor, 1);
                }

                var saveProgress = ApplicationController.Instance.SaveController.SaveProgress;
                if (saveProgress.IsCastleCompleted(castle.name))
                {
                    castle.ShowAsCompleted();
                }
                else
                {
                    var lastSessionProgress = ApplicationController.Instance.SaveController.SaveLastSessionProgress;
                
                    if (lastSessionProgress.IsValid() && castle.name == lastSessionProgress.Castle.Id)
                        castle.SetPoints(lastSessionProgress.Castle.Points, true);
                    else
                        castle.ShowAsLocked();
                }
            }
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
    
    public class UICastleLibraryPanelData : UIScreenData
    {
        public string Selected;
        public IEnumerable<Castle> Castles;
        public GameProcessor GameProcessor;
    }
}