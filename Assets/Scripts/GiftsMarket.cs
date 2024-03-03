using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Core
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
            OnCollect?.Invoke(success, sender.Id, sender.CurrencyAmount);
        }
    }
}