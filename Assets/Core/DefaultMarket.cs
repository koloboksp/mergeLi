using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Steps.CustomOperations;
using UnityEngine;

namespace Core
{
    public class DefaultMarket : MonoBehaviour, IMarket
    {
        public event Action<bool, string, int> OnBought;

        [SerializeField] private PurchasesLibrary _purchasesLibrary;
        
        public async Task<(bool success, int amount)> Buy(string productId, PurchaseType modelPurchaseType, CancellationToken cancellationToken)
        {
            var purchaseItem = _purchasesLibrary.Items.FirstOrDefault(i => string.Equals(i.ProductId, productId, StringComparison.Ordinal));
            if (purchaseItem == null)
            {
                Debug.LogError($"Can't buy product with id: '{productId}'. Product not found in library.");
                return (false, 0);
            }
            
            var success = true;
            var currencyAmount = 0;
            if (modelPurchaseType == PurchaseType.Market)
            {
                success = await ApplicationController.Instance.PurchaseController.Buy(purchaseItem.ProductId, cancellationToken);
                if (success)
                    currencyAmount = purchaseItem.CurrencyAmount;
            }
            else if (modelPurchaseType == PurchaseType.Ads)
            {
                success = await ApplicationController.Instance.AdsController.Show(AdvertisingType.Rewarded, cancellationToken);
                if (success)
                    currencyAmount = purchaseItem.CurrencyAmount;
            }
            
            OnBought?.Invoke(success, purchaseItem.ProductId, currencyAmount);
            
            return (success, currencyAmount);
        }
    }
}