using System;
using System.Threading.Tasks;
using Core.Steps.CustomOperations;

namespace Core
{
    public interface IMarket
    {
        public event Action<bool, string> OnBought;
        Task<bool> Buy(string productId, PurchaseType modelPurchaseType);
    }
}