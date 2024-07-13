using UnityEngine;

namespace Core
{
    public class GiftItem : MonoBehaviour
    {
        [SerializeField] private int _currencyAmount;
        [SerializeField] private long _collectInterval;
    
        public int CurrencyAmount => _currencyAmount;
        public long CollectInterval => _collectInterval;
    }
}