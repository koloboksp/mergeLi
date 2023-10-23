using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Core.Goals
{
    public class Castle : MonoBehaviour, ICastle
    {
        public event Action OnCompleted;
        public event Action OnPartSelected;
        public event Action OnProgressChanged;

        [SerializeField] private CastleViewer _view;
        [SerializeField] private int _coinsAfterComplete;
        
        private GameProcessor _gameProcessor;
        private readonly List<CastlePart> _parts = new();
        private int _points;
        private CastlePart _selectedPart;
        
        public string Id => gameObject.name;
        public bool Completed => _points >= GetCost(); 
        public CastleViewer View => _view;
        public IEnumerable<CastlePart> Parts => _parts;
        public int CoinsAfterComplete => _coinsAfterComplete;
        
        public CastlePart GetSelectedCastlePart()
        {
            return _selectedPart;
        }

        public int GetCost()
        {
            return _parts.Sum(i => i.Cost);
        }
        
        public int GetPoints()
        {
            return _points;
        }
        
        public void Init(GameProcessor gameProcessor)
        {
            _gameProcessor = gameProcessor;

            gameObject.GetComponentsInChildren(_parts);
            _parts.Sort((r, l) => r.Cost.CompareTo(l.Cost));
            
            _gameProcessor.OnScoreChanged += GameProcessor_OnScoreChanged;
            GameProcessor_OnScoreChanged1(0, true);
        }

        private void OnDestroy()
        {
            _gameProcessor.OnScoreChanged -= GameProcessor_OnScoreChanged;
        }

        private void GameProcessor_OnScoreChanged(int additionalPoints)
        {
            GameProcessor_OnScoreChanged1(additionalPoints, false);
        }

        private void GameProcessor_OnScoreChanged1(int additionalPoints, bool instant)
        {
            var completed = ProcessPoints(additionalPoints, instant);
            
            OnProgressChanged?.Invoke();

            if (completed)
                OnCompleted?.Invoke();
        }

        private bool ProcessPoints(int additionalPoints, bool instant)
        {
            _points += additionalPoints;

            var castleCost = _parts.Sum(i => i.Cost);
            var completed = false;
            if (_points >= castleCost)
            {
                completed = true;
                _points = castleCost;
            }
            
            ApplyPointsToParts(instant);

            return completed;
        }

        private void ApplyPointsToParts(bool instant)
        {
            var restScore = _points;
            var partUnlocked = true;
            CastlePart newSelectedPart = null;
            
            foreach (var part in _parts)
            {
                var consumePoints = part.Cost;
                
                part.ChangeUnlockState(partUnlocked, instant);
                
                if (restScore <= part.Cost)
                {
                    consumePoints = restScore;
                    partUnlocked = false;
                }
                
                part.SetPoints(consumePoints, instant);
                restScore -= consumePoints;

                if (restScore <= 0)
                {
                    newSelectedPart = part;
                    break;
                }
            }

            if (newSelectedPart != _selectedPart)
            {
                if(_selectedPart != null)
                    _selectedPart.Select(false);

                _selectedPart = newSelectedPart;
                
                if(_selectedPart != null)
                    _selectedPart.Select(true);
                
                OnPartSelected?.Invoke();
            }
        }

        public void SetPoints(int points, bool instant)
        {
            _points = points;
            GameProcessor_OnScoreChanged1(0, instant);
        }
        
        public void ResetPoints(bool instant)
        {
            _points = 0;
            GameProcessor_OnScoreChanged1(0, instant);
        }

        public void ForceComplete()
        {
            var requiredPoints = GetCost() - _points;
            ProcessPoints(requiredPoints, false);
        }

        public void ShowAsCompleted()
        {
            foreach (var part in _parts)
            {
                part.ChangeUnlockState(true, true);
                part.SetPoints(part.Cost, true);
            }
        }

        public async Task DestroyCastle(CancellationToken cancellationToken)
        {
            for (var index = _parts.Count - 1; index >= 0; index--)
            {
                var part = _parts[index];
                part.SetPoints(0, true);
                part.ChangeUnlockState(false, false);
            }

            await ApplicationController.WaitForSecondsAsync(2.0f, cancellationToken);
        }
    }
}