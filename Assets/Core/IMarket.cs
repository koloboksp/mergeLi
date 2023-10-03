using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Steps.CustomOperations;

namespace Core
{
    public interface IMarket
    {
        public event Action<bool, string> OnBought;
        Task<(bool success, int amount)> Buy(string productId, PurchaseType modelPurchaseType, CancellationToken cancellationToken);
    }
}