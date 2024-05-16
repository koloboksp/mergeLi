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
        [SerializeField] private UICastlesLibraryPanel_HiddenCastle _hiddenCastlePrefab;
        [SerializeField] private GameObject _castleSeparatorPrefab;
        [SerializeField] private int _hiddenCastlesCount = 3;
        
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

            var castlesData = new List<(Castle prefab, CastleViewType viewType, int points, int cost)>();
            for (var castleI = 0; castleI < castlesPrefabs.Count; castleI++)
            {
                var castlePrefab = castlesPrefabs[castleI];
                var castleViewType = CastleViewType.Locked;
                var castlePoints = 0;
                var castleCost = 0;

                var saveProgress = ApplicationController.Instance.SaveController.SaveProgress;
                if (saveProgress.IsCastleCompleted(castlePrefab.name))
                {
                    castleViewType = CastleViewType.Completed;
                    castlePoints = castleCost;
                }
                else
                {
                    if (lastActiveCastleName != null && castlePrefab.name == lastActiveCastleName)
                    {
                        castleViewType = CastleViewType.PartiallyReady;
                        castlePoints = lastActiveCastlePoints;
                    }
                }
                
                castlesData.Add((castlePrefab, castleViewType, castlePoints, castleCost));
            }

            var lastActiveCastleIndex = castlesData.FindLastIndex(i => i.viewType == CastleViewType.Completed 
                                                                       || i.viewType == CastleViewType.PartiallyReady);
            var showingCastleCount = lastActiveCastleIndex + _hiddenCastlesCount + 1;
            showingCastleCount = Mathf.Clamp(showingCastleCount, 0, castlesData.Count);

            for (var castleI = 0; castleI < showingCastleCount; castleI++)
            {
                var castleData = castlesData[castleI];
                
                var castleViewType = CastleViewType.Locked;
                var castlePoints = 0;
                var castleCost = 0; 
                
                var saveProgress = ApplicationController.Instance.SaveController.SaveProgress;
                if (saveProgress.IsCastleCompleted(castleData.prefab.Id))
                {
                    castleViewType = CastleViewType.Completed;
                    castlePoints = castleCost;
                }
                else
                {
                    if (lastActiveCastleName != null && castleData.prefab.Id == lastActiveCastleName)
                    {
                        castleViewType = CastleViewType.PartiallyReady;
                        castlePoints = lastActiveCastlePoints;
                    }
                }

                var castleContainer = new GameObject($"container_{castleData.prefab.Id}", typeof(RectTransform));
                var castleContainerTransform = castleContainer.GetComponent<RectTransform>();
                castleContainerTransform.SetParent(_container.content);
                castleContainerTransform.localScale = Vector3.one;
                castleContainerTransform.pivot = new Vector2(0, 1);
                
                if (castleViewType == CastleViewType.Locked)
                {
                    var hiddenCastle = Instantiate(_hiddenCastlePrefab, castleContainerTransform);
                    hiddenCastle.gameObject.name = castleData.prefab.Id;
                    
                    castleContainerTransform.sizeDelta = _hiddenCastlePrefab.Root.sizeDelta;
                }
                else
                {
                    var scaleFactor = 1.0f;
                    if (castleData.prefab.Root.sizeDelta.x > _container.content.sizeDelta.x)
                        scaleFactor = _container.content.sizeDelta.x / castleData.prefab.Root.sizeDelta.x;
                    
                    castleContainerTransform.sizeDelta = castleData.prefab.Root.sizeDelta * new Vector3(scaleFactor, scaleFactor, 1);

                    var castle = Instantiate(castleData.prefab, castleContainerTransform);
                    castle.gameObject.name = castleData.prefab.Id;
                    castle.SetData(_gameProcessor);
                    castle.Root.localScale = new Vector3(scaleFactor, scaleFactor, 1);

                    if (castleViewType == CastleViewType.Completed)
                        castle.ShowAsCompleted();
                    else
                        castle.SetPoints(lastActiveCastlePoints, true);
                    
                    focusOnCastleContainer = castleContainerTransform;
                    castleCost = castle.GetCost();
                }
                
                var castleLabel = Instantiate(_castleLabelPrefab, _container.content);
                castleLabel.gameObject.SetActive(true);
                castleLabel.SetData(castleData.prefab.NameKey, castleViewType, castlePoints, castleCost);

                if (castleI != showingCastleCount - 1)
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

        public enum CastleViewType 
        {
            Locked,
            PartiallyReady,
            Completed,
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