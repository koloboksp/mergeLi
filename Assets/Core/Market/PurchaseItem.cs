using UnityEngine;
using UnityEngine.Serialization;

namespace Core.Steps.CustomOperations
{
    public class PurchaseItem : MonoBehaviour
    {
        [FormerlySerializedAs("_inAppId")] [SerializeField] private string _productId;
        [SerializeField] private int _currencyAmount;
        [SerializeField] private PurchaseType _purchaseType;

        public string ProductId => _productId;
        public int CurrencyAmount => _currencyAmount;
        public PurchaseType PurchaseType => _purchaseType;
    }

    public enum PurchaseType
    {
        Market,
        Ads,
    }
}