using System;
using UnityEngine;

namespace Core.Effects
{
    public class PointsEffectReceiver : MonoBehaviour, IPointsEffectReceiver
    {
        [SerializeField] private Transform _anchor;
        
        public event Action<int> OnReceiveStart;
        public event Action<int> OnReceive;
        public event Action OnReceiveFinished;
        public event Action<int> OnRefund;

        public int Priority => 0;
        public Transform Anchor
        {
            get => _anchor == null ? transform : _anchor;
            set => _anchor = value;
        }

        public void ReceiveStart(int amount)
        {
            OnReceiveStart?.Invoke(amount);
        }
        
        public void Receive(int partAmount)
        {
            OnReceive?.Invoke(partAmount);
        }
        
        public void ReceiveFinished()
        {
            OnReceiveFinished?.Invoke();
        }

        public void Refund(int amount)
        {
            OnRefund?.Invoke(amount);
        }
    }
}