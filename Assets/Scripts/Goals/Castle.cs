using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Atom;
using Core.Effects;
using Unity.VisualScripting;
using UnityEngine;

namespace Core.Goals
{
    public class Castle : MonoBehaviour, ICastle
    {
        public event Action<Castle> OnCompleted;
        public event Action OnPartSelected;
     
        [SerializeField] private CastleViewer _view;
        [SerializeField] private int _coinsAfterComplete;
        [SerializeField] private GuidEx _nameKey;
        [SerializeField] private RectTransform _root;
        [SerializeField] private CoinsEffectReceiver _coinsEffectReceiver;

        private GameProcessor _gameProcessor;
        private readonly List<CastlePart> _parts = new();
        private bool _completed;
        private int _points;
        private CastlePart _selectedPart;
        
        public string Id => gameObject.name;
        public RectTransform Root => _root;

        public bool Completed => _points >= GetCost();
        public CastleViewer View => _view;
        public IEnumerable<CastlePart> Parts => _parts;
        public int CoinsAfterComplete => _coinsAfterComplete;
        public GuidEx NameKey => _nameKey;

        public CastlePart GetSelectedCastlePart()
        {
            return _selectedPart;
        }

        public int GetCost()
        {
            return _parts.Sum(i => i.Cost);
        }

        public int GetLastPartCost()
        {
            foreach (var part in _parts)
            {
                if (part.Points < part.Cost)
                    return part.Cost;
            }

            if (_parts.Count > 0)
            {
                return _parts[^1].Cost;
            }

            return int.MaxValue;
        }

        public int GetLastPartPoints()
        {
            foreach (var part in _parts)
            {
                if (part.Points < part.Cost)
                    return part.Points;
            }

            return 0;
        }


        public int GetPoints()
        {
            return _points;
        }

        public void SetData(GameProcessor gameProcessor)
        {
            _gameProcessor = gameProcessor;

            gameObject.GetComponentsInChildren(_parts);
            _parts.Sort((r, l) => r.Cost.CompareTo(l.Cost));

            _gameProcessor.OnScoreChanged += GameProcessor_OnScoreChanged;
            OnScoreChanged(0, true);

            _coinsEffectReceiver.OnReceive += CoinsEffectReceiver_OnReceive;
        }

        private void OnDestroy()
        {
            _gameProcessor.OnScoreChanged -= GameProcessor_OnScoreChanged;
        }

        private void GameProcessor_OnScoreChanged(int additionalPoints)
        {
            if (additionalPoints < 0)
                OnScoreChanged(additionalPoints, false);
        }

        private void OnScoreChanged(int additionalPoints, bool instant)
        {
            var oldCompletedState = _completed;
            _completed = ProcessPoints(additionalPoints, instant);

            if (oldCompletedState != _completed && _completed)
            {
                OnCompleted?.Invoke(this);
            }
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

            ApplyPointsToParts((int)Mathf.Sign(additionalPoints), instant);

            return completed;
        }

        private void ApplyPointsToParts(int direction, bool instant)
        {
            var partStates = new List<(int consumePoints, int cost, bool unlocked)>();

            var restScore = _points;
            var newSelectedPartIndex = 0;
            for (var partI = 0; partI < _parts.Count; partI++)
            {
                var part = _parts[partI];
                int consumePoints = restScore <= part.Cost
                    ? restScore
                    : part.Cost;

                var partUnlocked = partI == 0
                    ? true
                    : partStates[partI - 1].unlocked
                      && partStates[partI - 1].consumePoints == partStates[partI - 1].cost;

                if (partUnlocked)
                    newSelectedPartIndex = partI;

                partStates.Add((consumePoints, part.Cost, partUnlocked));

                restScore -= consumePoints;
            }

            if (direction >= 0)
            {
                for (var index = 0; index < _parts.Count; index++)
                {
                    var part = _parts[index];

                    var partState = partStates[index];
                    part.ChangeUnlockState(partState.unlocked, instant);
                    part.SetPoints(partState.consumePoints, instant);
                }
            }
            else
            {
                for (var index = _parts.Count - 1; index >= 0; index--)
                {
                    var part = _parts[index];

                    var partState = partStates[index];
                    part.SetPoints(partState.consumePoints, instant);
                    part.ChangeUnlockState(partState.unlocked, instant);
                }
            }

            var newSelectedPart = _parts[newSelectedPartIndex];
            if (newSelectedPart != _selectedPart)
            {
                if (_selectedPart != null)
                {
                    _selectedPart.Select(false);
                }

                _selectedPart = newSelectedPart;

                if (_selectedPart != null)
                {
                    _selectedPart.Select(true);
                    _coinsEffectReceiver.Anchor = _selectedPart.transform;
                }


                OnPartSelected?.Invoke();
            }
        }

        private void CoinsEffectReceiver_OnReceive(int amount)
        {
            OnScoreChanged(amount, false);
        }

        public void SetPoints(int points, bool instant)
        {
            _points = points;
            OnScoreChanged(0, instant);
        }

        public void ResetPoints(bool instant)
        {
            _points = 0;
            OnScoreChanged(0, instant);
        }

        public void ForceComplete()
        {
            var requiredPoints = GetCost() - _points;
            ProcessPoints(requiredPoints, false);
        }

        public void ShowAsLocked()
        {
            foreach (var part in _parts)
            {
                part.ChangeUnlockState(false, true);
                part.SetPoints(0, true);
            }

            _view.SetStage(0);
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
                if (index == 0)
                    part.ChangeUnlockState(true, true);
                else
                    part.ChangeUnlockState(false, true);
            }
        }
    }
}