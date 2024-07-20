using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Gameplay;
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
       
        [SerializeField] private UICastlesLibraryPanel_CastleItem _castleItemPrefab;
        [SerializeField] private GameObject _castleSeparatorPrefab;
        [SerializeField] private GameObject _castleTerraIncognitoPrefab;
        [SerializeField] private int _hiddenCastlesCount = 3;
        
        private Model _model;
        private GameProcessor _gameProcessor;
        private List<UICastlesLibraryPanel_CastleItem> _castleItems = new();
        
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

            var data = undefinedData as UICastleLibraryPanelData;
            _gameProcessor = data.GameProcessor;
            
            _model = new Model();
            _model.OnItemsUpdated += OnItemsUpdated;
                
            _model.SetData(data.Castles, data.Selected, _gameProcessor);
            OnItemsUpdated(_model.Castles);
            FocusOnSelected();
        }

        private void OnItemsUpdated(IReadOnlyList<UICastlesLibraryPanel_CastleItemModel> castles)
        {
            foreach (var castleItem in _castleItems)
                Destroy(castleItem.gameObject);
            _castleItems.Clear();
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(_container.content);
            
            _castleSeparatorPrefab.gameObject.SetActive(false);
            _castleTerraIncognitoPrefab.gameObject.SetActive(false);
            
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

            var castlesData = new List<(UICastlesLibraryPanel_CastleItemModel model, CastleViewType viewType, int points, int cost)>();
            for (var castleI = 0; castleI < castles.Count; castleI++)
            {
                var castle = castles[castleI];
                var castleViewType = CastleViewType.Locked;
                var castlePoints = 0;
                var castleCost = 0;

                var saveProgress = ApplicationController.Instance.SaveController.SaveProgress;
                if (saveProgress.IsCastleCompleted(castle.Id))
                {
                    castleViewType = CastleViewType.Completed;
                    castlePoints = castleCost;
                }
                else
                {
                    if (lastActiveCastleName != null && castle.Id == lastActiveCastleName)
                    {
                        castleViewType = CastleViewType.PartiallyReady;
                        castlePoints = lastActiveCastlePoints;
                    }
                }
                
                castlesData.Add((castle, castleViewType, castlePoints, castleCost));
            }

            var lastActiveCastleIndex = castlesData.FindLastIndex(i => i.viewType == CastleViewType.Completed 
                                                                       || i.viewType == CastleViewType.PartiallyReady);
            var showingCastleCount = lastActiveCastleIndex + _hiddenCastlesCount + 1;
            showingCastleCount = Mathf.Clamp(showingCastleCount, 0, castlesData.Count);

            for (var castleI = 0; castleI < showingCastleCount; castleI++)
            {
                var castleData = castlesData[castleI];
                var castleItem = Instantiate(_castleItemPrefab, _container.content);
                castleItem.name += $"_{castleData.model.Id}";
                castleItem.SetData(
                    castleData.model, 
                    lastActiveCastleName, 
                    lastActiveCastlePoints,
                    _container.content.sizeDelta.x);
                _castleItems.Add(castleItem);
               
                if (castleI != showingCastleCount - 1)
                {
                    var castleSeparator = Instantiate(_castleSeparatorPrefab, _container.content);
                    castleSeparator.gameObject.SetActive(true);
                }
                else
                {
                    var castleTerraIncognitoPrefab = Instantiate(_castleTerraIncognitoPrefab, _container.content);
                    castleTerraIncognitoPrefab.gameObject.SetActive(true);
                }
                
                LayoutRebuilder.ForceRebuildLayoutImmediate(_container.content);
            }
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(_container.content);
        }

        private void FocusOnSelected()
        {
            RectTransform focusOnCastleContainer = null;

            var focusedCastleItem = _castleItems.FirstOrDefault(i => i.Model.Id == _model.SelectedCastleId);
            if (focusedCastleItem != null)
            {
                focusOnCastleContainer = focusedCastleItem.Root;
            }
            
            if (focusOnCastleContainer != null)
            {
                var focusOnPosition = focusOnCastleContainer.anchoredPosition.y;
                var focusOnNormalPosition = focusOnPosition / (_container.content.rect.height - _container.viewport.rect.height);
                focusOnNormalPosition = 1.0f - Mathf.Clamp01(-focusOnNormalPosition);
                _container.normalizedPosition = new Vector2(0, focusOnNormalPosition);
            }
        }
        
        public void StartAutoScrollContent(float nSpeed)
        {
            StartCoroutine(ScrollContent(nSpeed));
        }

        private IEnumerator ScrollContent(float nSpeed)
        {
            _container.verticalNormalizedPosition = 1.0f;
            
            var scroll = true;
            while (scroll)
            {
                _container.verticalNormalizedPosition -= nSpeed * Time.deltaTime;
                yield return null;
                if(_container.verticalNormalizedPosition <= 0.0f)
                    break;
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
            public event Action<IReadOnlyList<UICastlesLibraryPanel_CastleItemModel>> OnItemsUpdated;
            
            private readonly List<UICastlesLibraryPanel_CastleItemModel> _castles = new ();
            public string _selectedCastleId;
            
            public IReadOnlyList<UICastlesLibraryPanel_CastleItemModel> Castles => _castles;
            public string SelectedCastleId => _selectedCastleId;
            
            public void SetData(IEnumerable<Castle> castles, string selectedCastle, GameProcessor gameProcessor)
            {
                foreach (var castle in castles)
                {
                    _castles.Add(
                        new UICastlesLibraryPanel_CastleItemModel(castle, gameProcessor));
                }
               
                TrySelect(selectedCastle);
            }

            internal void TrySelect(string newId)
            {
                _selectedCastleId = newId;
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