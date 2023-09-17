using UnityEngine;
using UnityEngine.Serialization;

namespace Core.Steps.CustomOperations
{
    public class PurchaseItem : MonoBehaviour
    {
        [SerializeField] private string _productId;
        [SerializeField] private int _currencyAmount;
        [SerializeField] private PurchaseType _purchaseType;
        [SerializeField] private string _backgroundName;

        public string ProductId => _productId;
        public int CurrencyAmount => _currencyAmount;
        public PurchaseType PurchaseType => _purchaseType;
        public string BackgroundName => _backgroundName;
    }

    public enum PurchaseType
    {
        Market,
        Ads,
    }
}