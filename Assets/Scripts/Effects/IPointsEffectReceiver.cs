using UnityEngine;

namespace Core.Effects
{
    public interface IPointsEffectReceiver
    {
        int Priority { get; }
        Transform Anchor { get; }
        
        void ReceiveStart(int amount);
        void Receive(int partAmount);
        void ReceiveFinished();
        void Refund(int amount);
    }
}