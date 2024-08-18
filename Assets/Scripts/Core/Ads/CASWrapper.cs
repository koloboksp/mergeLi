using System;
using System.Threading;
using System.Threading.Tasks;
using CAS;
using Core.Utils;
using UnityEngine;

namespace Core.Ads
{
    public class CASWrapper : IAdsController
    {
        private IMediationManager _manager;
        private bool _showing;
        private bool _errorsOnShowing;
        private float _waitTime = 3.0f;
        private IAdView _adView;
        
        public async Task InitializeAsync()
        {
            try
            {
                Debug.Log($"<color=#99ff99>Initialize {nameof(CASWrapper)}.</color>");
                var timer = new SmallTimer();
                _manager = Build();
                Subscribe();
                Debug.Log($"<color=#99ff99>Time initialize {nameof(CASWrapper)}: {timer.Update()}.</color>");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private IMediationManager Build()
        {
            return MobileAds.BuildManager()
                .WithInitListener((success, error) =>
                {
                    if (success)
                    {
                        Debug.Log($"CleverAdsSolutions version: {MobileAds.wrapperVersion}.");
                        Debug.Log($"CleverAdsSolutions SDK: {MobileAds.GetSDKVersion()}.");
                        Debug.Log($"<color=#00CCFF>CleverAdsSolutions initialize: success.</color>");
                    }
                    else
                    {
                        Debug.LogError($"CleverAdsSolutions init failed with error: '{error}'.");
                    }
                })
                .Build();
        }

        private void Subscribe()
        {
            _manager.OnInterstitialAdLoaded += ()=> { };
            _manager.OnInterstitialAdFailedToLoad += (error) => {};
            _manager.OnInterstitialAdOpening += (meta) => {};
            _manager.OnInterstitialAdFailedToShow += (error) =>
            {
                _showing = false;
                _errorsOnShowing = true;
            };
            _manager.OnInterstitialAdClicked += ()=> {};
            _manager.OnInterstitialAdClosed += () =>
            {
                _showing = false;
            };

            _manager.OnRewardedAdLoaded += () => { };
            _manager.OnRewardedAdFailedToLoad += (error) => { };
            _manager.OnRewardedAdOpening += (meta) => { };
            _manager.OnRewardedAdFailedToShow += (error) =>
            {
                _showing = false;
                _errorsOnShowing = true;
            };
            _manager.OnRewardedAdClicked += () => { };
            _manager.OnRewardedAdClosed += () =>
            {
                _showing = false;
            };
        }

        private bool IsReadyToShow(AdType advertisingType)
        {
            return _manager != null && _manager.IsReadyAd(advertisingType);
        }

        public async Task<bool> Show(AdvertisingType advertisingType, CancellationToken cancellationToken)
        {
            var adType = advertisingType switch
            {
                AdvertisingType.Interstitial => AdType.Interstitial,
                AdvertisingType.Rewarded => AdType.Rewarded,
                _ => AdType.None
            };

            var waitForTimeout = AsyncExtensions.WaitForSecondsAsync(_waitTime, cancellationToken);
            var waitForAdLoading = AsyncExtensions.WaitForConditionAsync(() => !IsReadyToShow(adType), cancellationToken);
            await Task.WhenAny(
                waitForTimeout,
                waitForAdLoading);

            if (waitForTimeout.IsCompleted)
                return false;
            
            _showing = true;
            _errorsOnShowing = false;
            
            _manager.ShowAd(adType);

            while (_showing)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await Task.Yield();
            }
            
            return !_errorsOnShowing;
        }

        public bool ShowBanner(Vector2Int position, BannerSize bannerSize)
        {
            try
            {
                AdSize adSize;
                switch (bannerSize)
                {
                    case BannerSize.Height50:
                        adSize = AdSize.Banner;
                        break;
                    case BannerSize.Height50_250:
                        adSize = AdSize.AdaptiveFullWidth;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(bannerSize), bannerSize, null);
                }

                if (_adView != null)
                {
                    if (_adView.size != adSize)
                    {
                        _adView.SetActive(false);
                        _adView.Dispose();
                        _adView = null;
                    }
                }

                if (_adView == null)
                {
                    _adView = _manager.GetAdView(adSize);
                }
                
                _adView.SetActive(true);
                _adView.SetPositionPx(position.x, position.y, AdPosition.BottomCenter);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
            
            return true;
        }

        public void HideBanner()
        {
            if (_adView != null)
            {
                _adView.SetActive(false);
                //_adView.Dispose();
            }
        }
    }
}