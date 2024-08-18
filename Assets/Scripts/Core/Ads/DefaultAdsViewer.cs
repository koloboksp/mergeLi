using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Ads
{
    public class DefaultAdsViewer : MonoBehaviour, IAdsViewer
    {
        private static readonly Vector3[] NoAllocBannerCorners = new Vector3[4];
        
        public event Action<bool, string, int> OnShowAds;

        [SerializeField] private AdsLibrary _library;
        [SerializeField] private bool _bannerEnable;
        [SerializeField] private Canvas _screenCanvas;

        public async Task<(bool success, int amount)> Show(string adsName, CancellationToken cancellationToken)
        {
            var purchaseItem = _library.Items.FirstOrDefault(i => string.Equals(i.Name, adsName, StringComparison.Ordinal));
            if (purchaseItem == null)
            {
                Debug.LogError($"Can't show ads with id: '{adsName}'. Product not found in library.");
                return (false, 0);
            }

            var success = true;
            var currencyAmount = 0;

            success = await ApplicationController.Instance.AdsController.Show(AdvertisingType.Rewarded, cancellationToken);
            if (success)
                currencyAmount = purchaseItem.CurrencyAmount;

            try
            {
                OnShowAds?.Invoke(success, adsName, currencyAmount);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            
            return (success, currencyAmount);
        }

        private List<AdsBanner> _adsBanners = new List<AdsBanner>();
        public void AddBanner(AdsBanner banner)
        {
            if (!_adsBanners.Contains(banner))
            {
                _adsBanners.Add(banner);
                UpdateBanners();
            }
        }

        public void RemoveBanner(AdsBanner banner)
        {
            if (_adsBanners.Remove(banner))
            {
                UpdateBanners();
            }
        }
        
        private void UpdateBanners()
        {
            if (_bannerEnable && _adsBanners.Count > 0)
            {
                _adsBanners[0].Root.GetWorldCorners(NoAllocBannerCorners);
                    
                var targetLeftBottomCorner =  _screenCanvas.worldCamera.WorldToScreenPoint(NoAllocBannerCorners[0]);
                var targetRightTopCorner = _screenCanvas.worldCamera.WorldToScreenPoint(NoAllocBannerCorners[2]);
                var bannerPosition = new Vector2Int(0, (int)targetLeftBottomCorner.y);
                
                var height = targetRightTopCorner.y - targetLeftBottomCorner.y;
                if (height >= 250)
                {
                    ApplicationController.Instance.AdsController.ShowBanner(bannerPosition, BannerSize.Height50_250);
                }
                else if (height >= 50)
                {
                    ApplicationController.Instance.AdsController.ShowBanner(bannerPosition, BannerSize.Height50);
                }
                else
                {
                    ApplicationController.Instance.AdsController.HideBanner();
                }
                Debug.LogError(height);
            }
            else
            {
                ApplicationController.Instance.AdsController.HideBanner();
            }
        }
    }
}