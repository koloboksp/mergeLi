using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Core.Goals
{
    public interface ICastle
    {
        int GetPoints();
        string Id { get; }
        bool Completed { get; }
    }
    public class Castle : MonoBehaviour, ICastle
    {
        public event Action OnCompleted;
        public event Action OnPartSelected;

        [SerializeField] private CastleView _view;
        [SerializeField] private CastlePart _partPrefab;
        
        private GameProcessor _gameProcessor;
        private readonly List<CastlePart> _parts = new();
        private int _points;
        private CastlePart _selectedPart;
        
        public string Id => gameObject.name;
        public bool Completed => _points >= GetCost(); 
        public CastleView View => _view;
        public IEnumerable<CastlePart> Parts => _parts;

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

            //var castleProgress = gameProcessor.PlayerInfo.GetCastleProgress(Id);
            //if(castleProgress != null)
            //    ApplyProgress(castleProgress);
            
            _gameProcessor.OnScoreChanged += GameProcessor_OnScoreChanged;
            GameProcessor_OnScoreChanged(0);
        }

        private void OnDestroy()
        {
            _gameProcessor.OnScoreChanged -= GameProcessor_OnScoreChanged;
        }

        private void GameProcessor_OnScoreChanged(int additionalPoints)
        {
            _points += additionalPoints;
            
            var castleCost = _parts.Sum(i => i.Cost);
            var completed = false;
            if (_points >= castleCost)
            {
                completed = true;
                _points = castleCost;
            }

            ApplyPointsToParts();
            
            if (completed)
            {
                OnCompleted?.Invoke();
            }
        }

      // public void ApplyProgress(CastleProgress castleProgress)
      // {
      //     if (castleProgress.IsCompleted)
      //     {
      //         foreach (var part in _parts)
      //         {
      //             part.Complete();
      //         }
      //     } 
      // }
        
        private void ApplyPointsToParts()
        {
            var restScore = _points;
            CastlePart newSelectedPart = null;
            foreach (var part in _parts)
            {
                var consumePoints = part.Cost;
                if (restScore <= part.Cost)
                    consumePoints = restScore;
               
                part.SetPoints(consumePoints);
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

        public void SetPoints(int points)
        {
            _points = points;
            GameProcessor_OnScoreChanged(0);
        }
        
        public void ResetPoints()
        {
            _points = 0;
            GameProcessor_OnScoreChanged(0);
        }
    }
}