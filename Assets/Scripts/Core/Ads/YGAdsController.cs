#if UNITY_WEBGL
using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Utils;
using UnityEngine;
using YG;

namespace Core.Ads
{
    public class YGAdsController : IAdsController
    {
        private TaskCompletionSource<bool> _showCompletionSource;

        public Task InitializeAsync()
        {
            Debug.Log($"YGAdsController initialized.");
            
            return Task.CompletedTask;
        }
        
        public async Task<bool> Show(AdvertisingType adType, CancellationToken cancellationToken)
        {
            YG2.onRewardAdv += OnReward;
            YG2.RewardedAdvShow(adType.ToString());

            _showCompletionSource = new TaskCompletionSource<bool>();
            var cancellationTokenRegistration = cancellationToken.Register(() => _showCompletionSource.TrySetCanceled(cancellationToken));

            try
            {
                var showResult = await _showCompletionSource.Task;
                return showResult;
            }
            finally
            {
                YG2.onRewardAdv -= OnReward;
                cancellationTokenRegistration.Dispose();
            }
        }
        
        void OnReward(string id)
        {
            _showCompletionSource.SetResult(true);
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
            return true;
        }
    }
}
#endif