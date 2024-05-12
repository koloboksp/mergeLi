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
using UnityEngine.Serialization;

namespace Core.Goals
{
    public class Castle : MonoBehaviour, ICastle
    {
        public event Action<int> OnPointsAdd;
        public event Action<int> OnPointsRefund;
     
        [SerializeField] private CastleViewer2 _view;
        [SerializeField] private int initalCostOfPart;
        [SerializeField] private int risingCostOfPart = 100;

        [SerializeField] private int _coinsAfterComplete;
        [SerializeField] private GuidEx _nameKey;
        [SerializeField] private RectTransform _root;
        [SerializeField] private PointsEffectReceiver _pointsReceiver;

        [SerializeField] private GuidEx _textOnBuildStartingKey;
        [FormerlySerializedAs("_textOnBuildEndingKey")] [SerializeField] private GuidEx _textAfterBuildEndingKey;

        private GameProcessor _gameProcessor;
        private readonly List<CastlePart> _parts = new();
        private bool _completed;
        private int _points;
        private int _animatedPoints;
        private CastlePart _selectedPart;
        private bool _inPointsReceiveState;
        private TaskCompletionSource<bool> _pointsReceiveCompletedSource;

        public string Id => gameObject.name;
        public RectTransform Root => _root;

        public bool Completed => _points >= GetCost();
        public CastleViewer2 View => _view;
        public IEnumerable<CastlePart> Parts => _parts;
        public int CoinsAfterComplete => _coinsAfterComplete;
        public GuidEx NameKey => _nameKey;
        public GuidEx TextOnBuildStartingKey => _textOnBuildStartingKey;
        public GuidEx TextAfterBuildEndingKey => _textAfterBuildEndingKey;

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

            return GetCost();
        }


        public int GetPoints()
        {
            return _points;
        }
        
        public void SetData(GameProcessor gameProcessor)
        {
            _gameProcessor = gameProcessor;

            _parts.Clear();
            
            var castleBits = gameObject.GetComponentsInChildren<CastleBit>();
            castleBits = castleBits.Reverse().ToArray();
            for (var castleBitI = 0; castleBitI < castleBits.Length; castleBitI++)
            {
                var castleBit = castleBits[castleBitI];
                var castlePart = castleBit.gameObject.AddComponent<CastlePart>();
                castlePart.Owner = this;
                castlePart.Index = castleBitI;
                castlePart.Cost = initalCostOfPart + risingCostOfPart * castleBitI;
               
                var castlePartView = castleBit.gameObject.AddComponent<CastlePartView>();
                castlePartView.SetData(castlePart);
                
                _parts.Add(castlePart);
            }

            _parts.Sort((r, l) => r.Cost.CompareTo(l.Cost));

            _points = 0;
            _completed = false;
            ApplyPointsToParts(Order.Increase, true);

            _pointsReceiver.OnReceiveStart += PointsReceiver_OnReceiveStart; 
            _pointsReceiver.OnReceive += PointsReceiver_OnReceive;
            _pointsReceiver.OnReceiveFinished += PointsReceiver_OnReceiveFinished;
            _pointsReceiver.OnRefund += PointsReceiver_OnRefund;
        }
        
        private void PointsReceiver_OnReceiveStart(int amount)
        {
            _points += amount;

            _inPointsReceiveState = true;
        }

        private void PointsReceiver_OnReceive(int partAmount)
        {
            _animatedPoints += partAmount;
            ApplyPointsToParts(Order.Increase, false);

            OnPointsAdd?.Invoke(partAmount);
        }
        
        private void PointsReceiver_OnReceiveFinished()
        {
            var oldCompletedState = _completed;
            _completed = UpdateCompleteState();
            
            _inPointsReceiveState = false;
            
            if (_pointsReceiveCompletedSource != null)
                _pointsReceiveCompletedSource.TrySetResult(true);
        }
        
        void PointsReceiver_OnRefund(int refundPoints)
        {
            _points -= refundPoints;
            _animatedPoints = _points;
            _completed = UpdateCompleteState();
            ApplyPointsToParts(Order.Decrease, false);

            OnPointsRefund?.Invoke(refundPoints);
        }
        
        private bool UpdateCompleteState()
        {
            var castleCost = _parts.Sum(i => i.Cost);
            var completed = _points >= castleCost;
            
            return completed;
        }
        
        private void ApplyPointsToParts(Order order, bool instant)
        {
            var partStates = new List<(int consumePoints, int cost, bool unlocked)>();

            var restScore = _animatedPoints;
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

            if (order == Order.Increase)
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
                    _pointsReceiver.Anchor = _selectedPart.transform;
                }
            }
        }

        public void SetPoints(int points, bool instant)
        {
            _points = points;
            _animatedPoints = _points;
            _completed = UpdateCompleteState();
            ApplyPointsToParts(Order.Increase, instant);
        }

        public void ResetPoints(bool instant)
        {
            _points = 0;
            _animatedPoints = _points;
            _completed = UpdateCompleteState();
            ApplyPointsToParts(Order.Increase, instant);
        }

        public void ForceComplete()
        {
            var oldPointsValue = _points;
            var cost = GetCost();
            if (_points < cost)
                _points = cost;
            
            _animatedPoints = oldPointsValue;
            _completed = UpdateCompleteState();
            ApplyPointsToParts(Order.Increase, false);
        }

        public void ShowAsLocked()
        {
            foreach (var part in _parts)
            {
                part.SetPoints(0, true);
                part.ChangeUnlockState(false, true);
            }

            _view.ResetProgress();
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

        public int RestPoints()
        {
            return _points - GetCost();
        }
        
        public enum Order
        {
            Increase,
            Decrease,
        }

        public async Task WaitForAnimationsComplete(CancellationToken exitToken)
        {
            await _view.WaitForAnimationsComplete(exitToken);
        }
        
        public async Task WaitForCoinsReceiveEffectComplete(CancellationToken exitToken)
        {
            if (!_inPointsReceiveState)
                return;
            
            var exitTokenCompletion = new TaskCompletionSource<bool>();
            var exitTokenRegistration = exitToken.Register(() => exitTokenCompletion.TrySetCanceled(exitToken));
        
            try
            {
                if (_pointsReceiveCompletedSource == null)
                    _pointsReceiveCompletedSource = new TaskCompletionSource<bool>();

                await Task.WhenAny(_pointsReceiveCompletedSource.Task, exitTokenCompletion.Task);

                _pointsReceiveCompletedSource = null;
            }
            finally
            {
                exitTokenRegistration.Dispose();
            } 
        }

        public async Task PlayBuildingComplete()
        {
            await _view.PlayBuildingComplete();
        }
    }
}