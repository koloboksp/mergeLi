using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Ads
{
    public class DefaultAdsViewer : MonoBehaviour, IAdsViewer
    {
        public event Action<bool, string, int> OnShowAds;

        [SerializeField] private AdsLibrary _library;
        
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
    }
}