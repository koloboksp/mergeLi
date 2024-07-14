using UnityEngine;

namespace Core.Market
{
    public class PurchaseItem : MonoBehaviour
    {
        [SerializeField] private string _productId;
        [SerializeField] private int _currencyAmount;
        [SerializeField] private string _backgroundName;
        [SerializeField] private Sprite _icon;

        public string ProductId => _productId;
        public int CurrencyAmount => _currencyAmount;
        public string BackgroundName => _backgroundName;
        public Sprite Icon => _icon;
    }
}