using System;
using System.Threading;
using System.Threading.Tasks;
using CAS;
using UnityEngine;

namespace Core
{
    public class CASWrapper : IAdsController
    {
        IMediationManager _manager;
        bool _showing;
        bool _errorsOnShowing;
        
        public async Task InitializeAsync()
        {
            var timer = new SmallTimer();
            
            _manager = Build();
            Subscribe();
            Debug.Log($"<color=#99ff99>Time initialize {nameof(CASWrapper)}: {timer.Update()}.</color>");
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

        private bool IsReadyToShow()
        {
            return _manager != null && _manager.IsReadyAd(AdType.Interstitial);
        }

        public async Task<bool> Show(AdvertisingType advertisingType, CancellationToken cancellationToken)
        {
            if (!IsReadyToShow())
                await Task.CompletedTask;

            _showing = true;
            _errorsOnShowing = false;

            var adType = advertisingType switch
            {
                AdvertisingType.Interstitial => AdType.Interstitial,
                AdvertisingType.Rewarded => AdType.Rewarded,
                _ => AdType.None
            };
            
            _manager.ShowAd(adType);

            while (_showing)
                await Task.Yield();

            return !_errorsOnShowing;
        }
    }
}