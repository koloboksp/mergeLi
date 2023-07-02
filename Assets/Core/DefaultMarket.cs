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
        
        public async Task<bool> Buy(string inAppId)
        {
            var purchaseItem = _purchasesLibrary.Items.FirstOrDefault(i => string.Equals(i.InAppId, inAppId, StringComparison.Ordinal));

            bool result = true;
            OnBought?.Invoke(result, purchaseItem.InAppId);
            
            return result;
        }
    }
}