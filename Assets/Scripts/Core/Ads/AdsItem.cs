using UnityEngine;

namespace Core.Ads
{
    public class AdsItem : MonoBehaviour
    {
        [SerializeField] private int _currencyAmount;
        [SerializeField] private AdvertisingType _advertisingType;
        
        public string Name => name;
        public int CurrencyAmount => _currencyAmount;
        public AdvertisingType AdvertisingType => _advertisingType;
    }
}