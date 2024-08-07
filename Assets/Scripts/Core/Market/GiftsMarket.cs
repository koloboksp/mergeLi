using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Market
{
    public class GiftsMarket : MonoBehaviour
    {
        public event Action<bool, string, int> OnCollect;

        [SerializeField] private GiftsLibrary _giftsLibrary;

        private readonly List<GiftModel> _gifts = new List<GiftModel>();

        public IReadOnlyList<GiftModel> Gifts => _gifts;
        
        public void Initialize()
        {
            foreach (var item in _giftsLibrary.Items)
            {
                var gift = new GiftModel(item.name, item.CollectInterval, item.CurrencyAmount,
                    ApplicationController.Instance.SaveController.SaveProgress);
                gift.OnCollect += Gift_OnCollect;
                _gifts.Add(gift);
            }
        }

        private void Gift_OnCollect(GiftModel sender, bool success)
        {
            try
            {
                OnCollect?.Invoke(success, sender.Id, sender.CurrencyAmount);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}