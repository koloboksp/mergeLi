using System;
using UnityEngine;

namespace Core.Effects
{
    public interface IPointsEffectReceiver
    {
        int Priority { get; }
        Transform Anchor { get; }
        
        void Receive(int amount);
    }
    
    public class CoinsEffectReceiver : MonoBehaviour, IPointsEffectReceiver
    {
        [SerializeField] private Transform _anchor;
        
        public event Action<int> OnReceive;
        
        public int Priority => 0;
        public Transform Anchor
        {
            get => _anchor == null ? transform : _anchor;
            set => _anchor = value;
        }

        public void Receive(int amount)
        {
            OnReceive?.Invoke(amount);
        }
    }
}