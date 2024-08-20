using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Ads
{
    public class DefaultAdsController : IAdsController
    {
        public Task InitializeAsync()
        {
            Debug.Log($"DefaultAdsController initialized.");
            return Task.CompletedTask;
        }

        public Task<bool> Show(AdvertisingType adType, CancellationToken cancellationToken)
        {
            return Task.FromResult(false);
        }

        public bool ShowBanner(Vector2Int position, BannerSize bannerSize)
        {
            return false;
        }

        public void HideBanner()
        {
        }

        public bool IsAdsAvailable(string adsId, AdvertisingType advertisingType)
        {
            return false;
        }
    }
}