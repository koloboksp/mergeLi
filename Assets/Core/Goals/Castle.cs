using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Core.Goals
{
    public class Castle : MonoBehaviour
    {
        public event Action OnCompleted;
        public event Action OnPartSelected;

        [SerializeField] private CastleView _view;
        [SerializeField] private CastlePart _partPrefab;
        
        private GameProcessor _gameProcessor;
        private List<CastlePart> _parts = new();
        private int _selectedPartIndex;
    
        public string Name => gameObject.name;
        public CastleView View => _view;

        public CastlePart SelectedCastlePart
        {
            get
            {
                if (_selectedPartIndex < 0 || _selectedPartIndex >= _parts.Count) return null;
                
                return _parts[_selectedPartIndex];
            }
        }
        
        public void Init(GameProcessor gameProcessor)
        {
            _gameProcessor = gameProcessor;

            this.gameObject.GetComponentsInChildren<CastlePart>(_parts);
            
            var castleProgress = gameProcessor.PlayerInfo.GetCastleProgress(Name);
            if(castleProgress != null)
                ApplyProgress(castleProgress);
            
            _gameProcessor.OnScoreChanged += GameProcessor_OnScoreChanged;
            GameProcessor_OnScoreChanged(0);
            
            SelectDefaultPart();
        }

        private void OnDestroy()
        {
            _gameProcessor.OnScoreChanged -= GameProcessor_OnScoreChanged;
        }

        private void GameProcessor_OnScoreChanged(int additionalPoints)
        {
            AddProgress(additionalPoints);

            var castleProgress = new CastleProgress()
            {
                Name = Name,
            };
            foreach (var part in _parts)
            {
                castleProgress.Parts.Add(new CastlePartProgress()
                {
                    GridPosition = part.GridPosition, 
                    IsCompleted = part.IsCompleted,
                });
            }
            castleProgress.SelectedPartIndex = _selectedPartIndex;
            var isCompleted = _parts.All(i => i.IsCompleted);
            castleProgress.IsCompleted = isCompleted;
            
            _gameProcessor.PlayerInfo.SetCastleProgress(castleProgress);
            
            if(isCompleted)
                OnCompleted?.Invoke();
        }

        public void ApplyProgress(CastleProgress castleProgress)
        {
            _selectedPartIndex = castleProgress.SelectedPartIndex;
        
            foreach (var partProgress in castleProgress.Parts)
            {
                var foundPart = _parts.Find(i => i.GridPosition == partProgress.GridPosition);
                if (foundPart != null)
                    foundPart.ApplyProgress(partProgress);
            }

            _parts[_selectedPartIndex].Select(true);
        }

        public void SelectPart(CastlePart newSelected)
        {
            _selectedPartIndex = _parts.IndexOf(newSelected);
            for (var partI = 0; partI < _parts.Count; partI++)
            {
                var part = _parts[partI];
                part.Select(partI == _selectedPartIndex);
            }
            
            OnPartSelected?.Invoke();
        }

        private void AddProgress(int additionalProgress)
        {
            additionalProgress = _parts[_selectedPartIndex].AddPoints(additionalProgress);
            while (additionalProgress > 0)
            {
                int minCost = int.MaxValue;
                CastlePart newSelectedPart = null;
                foreach (var part in _parts)
                    if (!part.IsCompleted && part.Points < minCost)
                        newSelectedPart = part;

                if (newSelectedPart != null)
                {
                    additionalProgress = newSelectedPart.AddPoints(additionalProgress);
                    SelectPart(newSelectedPart);
                }
                else
                    break;
            }
        }
        
        public void SelectDefaultPart()
        {
            CastlePart newSelectedPart = null;
            int minCost = int.MaxValue;
            foreach (var part in _parts)
                if (!part.IsCompleted && part.Points < minCost)
                {
                    minCost = Mathf.Min(minCost, part.Points);
                    newSelectedPart = part;
                }

            SelectPart(newSelectedPart);
        }
    }
}