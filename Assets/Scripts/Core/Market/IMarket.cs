using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Market
{
    public interface IMarket
    {
        public event Action<bool, string, int> OnBought;
        Task<(bool success, int amount)> BuyAsync(string productId, CancellationToken cancellationToken);
    }
}