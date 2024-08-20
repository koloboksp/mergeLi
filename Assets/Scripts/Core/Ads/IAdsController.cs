using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Ads
{
    public interface IAdsController
    {
        Task InitializeAsync();
        Task<bool> Show(AdvertisingType adType, CancellationToken cancellationToken);
        bool ShowBanner(Vector2Int position, BannerSize bannerSize);
        void HideBanner();
        bool IsAdsAvailable(string adsId, AdvertisingType advertisingType);
    }
    
    public enum AdvertisingType
    {
        Rewarded,
        Interstitial,
    }
    
    public enum BannerSize
    {
        Height50,
        Height50_250,
    }
}