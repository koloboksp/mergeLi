using UnityEngine;

namespace Core
{
    public interface ICoinsReceiver
    {
        Transform Anchor { get; }
        bool IsActive { get; }
        void Receive(int amount);
    }
}