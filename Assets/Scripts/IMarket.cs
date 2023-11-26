using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Steps.CustomOperations;

namespace Core
{
    public interface IMarket
    {
        public event Action<bool, string, int> OnBought;
        Task<(bool success, int amount)> BuyAsync(string productId, CancellationToken cancellationToken);
    }
}