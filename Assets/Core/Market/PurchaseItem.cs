using UnityEngine;

namespace Core.Steps.CustomOperations
{
    public class PurchaseItem : MonoBehaviour
    {
        [SerializeField] private string _inAppId;
        [SerializeField] private int _currencyAmount;

        public string InAppId => _inAppId;
        public int CurrencyAmount => _currencyAmount;
    }
}