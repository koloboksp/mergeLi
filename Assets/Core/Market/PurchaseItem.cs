using UnityEngine;

namespace Core.Steps.CustomOperations
{
    public class PurchaseItem : MonoBehaviour
    {
        [SerializeField] private string _inAppId;
        [SerializeField] private int _currencyAmount;
        [SerializeField] private PurchaseType _purchaseType;

        public string InAppId => _inAppId;
        public int CurrencyAmount => _currencyAmount;
        public PurchaseType PurchaseType => _purchaseType;
    }

    public enum PurchaseType
    {
        Market,
        Ads,
    }
}