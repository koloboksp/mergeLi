using System;
using System.Linq;
using System.Threading.Tasks;
using Core.Steps.CustomOperations;
using UnityEngine;

namespace Core
{
    public class DefaultMarket : MonoBehaviour, IMarket
    {
        public event Action<bool, string> OnBought;

        [SerializeField] private PurchasesLibrary _purchasesLibrary;
        
        public async Task<bool> Buy(string productId, PurchaseType modelPurchaseType)
        {
            var purchaseItem = _purchasesLibrary.Items.FirstOrDefault(i => string.Equals(i.ProductId, productId, StringComparison.Ordinal));
            if (purchaseItem == null)
            {
                Debug.LogError($"Can't buy product with id: '{productId}'. Product not found in library.");
                return false;
            }
            
            var result = true;
            if (modelPurchaseType == PurchaseType.Market)
            {
                result = await ApplicationController.Instance.PurchaseController.Buy(purchaseItem.ProductId);
            }
            else
            {
                
            }
            
            OnBought?.Invoke(result, purchaseItem.ProductId);
            
            return result;
        }
    }
}