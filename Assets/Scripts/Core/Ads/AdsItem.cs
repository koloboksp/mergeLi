using UnityEngine;

namespace Core.Ads
{
    public class AdsItem : MonoBehaviour
    {
        [SerializeField] private int _currencyAmount;

        public string Name => name;
        public int CurrencyAmount => _currencyAmount;
    }
}