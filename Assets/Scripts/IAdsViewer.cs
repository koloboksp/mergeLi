using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core
{
    public interface IAdsViewer
    {
        public event Action<bool, string, int> OnShowAds;
        Task<(bool success, int amount)> Show(string productId, CancellationToken cancellationToken);
    }
}