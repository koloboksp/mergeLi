using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Goals;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI.Panels
{
    public class UICastlesLibraryPanel : UIPanel
    {
        [SerializeField] private Button _closeBtn;
        [SerializeField] private ScrollRect _container;
       
        [SerializeField] private UICastlesLibraryPanel_CastleLabel _castleLabelPrefab;
        [SerializeField] private GameObject _castleSeparatorPrefab;

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
            
            _model = new Model();
            _model.OnItemsUpdated += OnItemsUpdated;
                
            _model.SetData(data.Castles, data.Selected);
            OnItemsUpdated(_model.Castles);
        }

        private void OnItemsUpdated(IReadOnlyList<Castle> castlesPrefabs)
        {
            var oldViews = _container.content.GetComponents<Castle>();
            foreach (var oldView in oldViews)
                Destroy(oldView.gameObject);
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(_container.content);
            
            _castleLabelPrefab.gameObject.SetActive(false);
            _castleSeparatorPrefab.gameObject.SetActive(false);

            RectTransform focusOnCastleContainer = null;

            string lastActiveCastleName = null;
            var lastActiveCastlePoints = 0;
            
            if (_gameProcessor.SessionProcessor.HasPreviousSessionGame)
            {
                var lastSessionProgress = ApplicationController.Instance.SaveController.SaveLastSessionProgress;
                
                lastActiveCastleName = lastSessionProgress.ActiveCastle.Id;
                lastActiveCastlePoints = lastSessionProgress.ActiveCastle.Points;
            }
            else
            {
                lastActiveCastleName = _gameProcessor.SessionProcessor.GetFirstUncompletedCastleName();
                lastActiveCastlePoints = 0;
            }
            

            for (var castleI = 0; castleI < castlesPrefabs.Count; castleI++)
            {
                var castlePrefab = castlesPrefabs[castleI];
                
                var scaleFactor = 1.0f;
                if (castlePrefab.Root.sizeDelta.x > _container.content.sizeDelta.x)
                {
                    scaleFactor = _container.content.sizeDelta.x / castlePrefab.Root.sizeDelta.x;
                }

                var castleContainer = new GameObject($"container_{castlePrefab.Id}", typeof(RectTransform));
                var castleContainerTransform = castleContainer.GetComponent<RectTransform>();
                castleContainerTransform.SetParent(_container.content);
                castleContainerTransform.localScale = Vector3.one;
                castleContainerTransform.pivot = new Vector2(0, 1);
                castleContainerTransform.sizeDelta = castlePrefab.Root.sizeDelta * new Vector3(scaleFactor, scaleFactor, 1);

                var castle = Instantiate(castlePrefab, castleContainerTransform);
                castle.gameObject.name = castlePrefab.Id;
                castle.SetData(_gameProcessor);
                castle.Root.localScale = new Vector3(scaleFactor, scaleFactor, 1);

                var castlePoints = 0;
                var castleCost = castle.GetCost();
                
                var saveProgress = ApplicationController.Instance.SaveController.SaveProgress;
                if (saveProgress.IsCastleCompleted(castle.name))
                {
                    castle.ShowAsCompleted();
                    castlePoints = castleCost;
                    focusOnCastleContainer = castleContainerTransform;
                }
                else
                {
                    if (lastActiveCastleName != null && castle.name == lastActiveCastleName)
                    {
                        castle.SetPoints(lastActiveCastlePoints, true);
                        castlePoints = lastActiveCastlePoints;
                        focusOnCastleContainer = castleContainerTransform;
                    }
                    else
                    {
                        castlePoints = 0;
                        castle.ShowAsLocked();
                    }
                }

                var castleLabel = Instantiate(_castleLabelPrefab, _container.content);
                castleLabel.gameObject.SetActive(true);
                castleLabel.SetData(castlePrefab.NameKey, castlePoints, castleCost);

                if (castleI != castlesPrefabs.Count - 1)
                {
                    var castleSeparator = Instantiate(_castleSeparatorPrefab, _container.content);
                    castleSeparator.gameObject.SetActive(true);
                }
            }
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(_container.content);
            
            if (focusOnCastleContainer != null)
            {
                var focusOnPosition = focusOnCastleContainer.anchoredPosition.y;
                var focusOnNormalPosition = focusOnPosition / (_container.content.rect.height - _container.viewport.rect.height);
                focusOnNormalPosition = 1.0f - Mathf.Clamp01(-focusOnNormalPosition);
                _container.normalizedPosition = new Vector2(0, focusOnNormalPosition);
            }
        }
      
        public class Model
        {
            public event Action<IReadOnlyList<Castle>> OnItemsUpdated;
            
            private readonly List<Castle> _castles = new ();

            public IReadOnlyList<Castle> Castles => _castles;
            
            public void SetData(IEnumerable<Castle> castles, string selectedCastle)
            {
                _castles.AddRange(castles);
              
                TrySelect(_castles.Find(i => i.Id == selectedCastle));
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