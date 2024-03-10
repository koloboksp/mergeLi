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
        
        [SerializeField] private UIUpScaleEffect _iconUpScaleEffect;
        [SerializeField] private int _effectPriority;

        public void SetSessionScore(int sessionScore, int bestSessionScore)
        {
            _sessionScoreLabel.text = sessionScore.ToString();
            _bestScoreLabel.text = bestSessionScore.ToString();
        }
        
        public void InstantSet(int points, int maxPoints)
        {
            _partProgressBar.InstantSet(points, maxPoints);
        }
        
        public void Set(float duration, int oldPoints, int newPoints, int maxPoints)
        {
            _partProgressBar.Set(duration, oldPoints, newPoints, maxPoints);
        }
        
        public void InstantComplete()
        {
            
        }

        public void Complete(float duration)
        {
           
        }
        
        public int Priority => _effectPriority;
        public Transform Anchor => _iconUpScaleEffect.Root;
      
        public void Receive(int amount)
        {
            _iconUpScaleEffect.Add();
        }
    }
}