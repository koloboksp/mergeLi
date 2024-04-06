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
        public event Action<int> OnReceive;
        
        public int Priority => 0;
        public Transform Anchor => transform;
        public void Receive(int amount)
        {
            OnReceive?.Invoke(amount);
        }
    }
}