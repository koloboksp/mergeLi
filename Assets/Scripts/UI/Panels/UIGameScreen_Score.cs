using System.Collections.Generic;
using Core.Effects;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Core
{
    public class UIGameScreen_Score : MonoBehaviour, IPointsEffectReceiver
    {
        [SerializeField] private Text _sessionScoreLabel;
        [SerializeField] private Text _bestScoreLabel;
        [SerializeField] private UIProgressBar _partProgressBar;
        [SerializeField] private UIScore _sessionScore;

        [SerializeField] private UIUpScaleEffect _iconUpScaleEffect;
        [SerializeField] private int _effectPriority;
        
        public void InstantSet(int points, int maxPoints)
        {
            _partProgressBar.InstantSet(points, maxPoints);
        }

        public void InstantSetSession(int sessionPoints, int previousSessionPoints)
        {
            _sessionScore.InstantSet(sessionPoints, previousSessionPoints);
        }
        
        public void Set(float duration, 
            int oldPoints,
            int newPoints, 
            int maxPoints)
        {
            _partProgressBar.Set(duration, oldPoints, newPoints, maxPoints);
        }
        
        public void SetSession(float duration, 
            int sessionOldPoints,
            int sessionNewPoints, 
            int previousSessionPoints,
            bool instant)
        {
            if (instant)
                _sessionScore.InstantSet(sessionNewPoints, previousSessionPoints);
            else
                _sessionScore.Set(duration, sessionOldPoints, sessionNewPoints, previousSessionPoints);
        }
        
        public int Priority => _effectPriority;
        public Transform Anchor => _iconUpScaleEffect.Root;

        public void ReceiveStart(int amount)
        {
            
        }

        public void Receive(int partAmount)
        {
            _iconUpScaleEffect.Add();
        }

        public void ReceiveFinished()
        {
            
        }

        public void Refund(int amount)
        {
            
        }
    }
}