using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Ads
{
    public interface IAdsViewer
    {
        public event Action<bool, string, int> OnShowAds;
        Task<(bool success, int amount)> Show(string productId, CancellationToken cancellationToken);
        void AddBanner(AdsBanner banner);
        void RemoveBanner(AdsBanner adsBanner);
    }
}