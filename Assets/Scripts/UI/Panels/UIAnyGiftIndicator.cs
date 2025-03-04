﻿using System;
using System.Threading;
using Core.Market;
using Core.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class UIAnyGiftIndicator : MonoBehaviour
    {
        public event Action OnClick;

        [SerializeField] private Button _areaButton;
        [SerializeField] private Animation _presentAnimation;
        [SerializeField] private AnimationClip _presentAvailableClip;
        [SerializeField] private AnimationClip _presentNotAvailableClip;
        [SerializeField] private UITimer _timer;
        [SerializeField] private GameObject _noInternetIcon;

        private GiftsMarket _giftsMarket;
       
        private void Awake()
        {
            _areaButton.onClick.AddListener(() => OnClick?.Invoke());
            _timer.OnComplete += Timer_OnComplete;
        }
        
        public void Set(GiftsMarket giftsMarket)
        {
            _giftsMarket = giftsMarket;
            
            SetAvailability();
        }

        private void SetAvailability()
        {
            var minRestTimeForCollect = Int64.MaxValue;
            if (_giftsMarket.Gifts.Count == 0)
            {
                gameObject.SetActive(false);
                return;
            }
            
            gameObject.SetActive(true);
            
            foreach (var gift in _giftsMarket.Gifts)
            {
                var restTimeForCollect = gift.GetRestTimeForCollect(NetworkTimeManager.NowTicks);
                minRestTimeForCollect = Math.Min(restTimeForCollect, minRestTimeForCollect);
            }
            
            if (NetworkTimeManager.TimeUpdated)
            {
                _noInternetIcon.gameObject.SetActive(false);

                var available = minRestTimeForCollect <= 0;
                if (!available)
                {
                    _timer.Set(minRestTimeForCollect);
                }
                
                _timer.gameObject.SetActive(!available);
                _presentAnimation.Play(available ? _presentAvailableClip.name : _presentNotAvailableClip.name);
            }
            else
            {
                _timer.gameObject.SetActive(false);
                _noInternetIcon.gameObject.SetActive(true);
                
                _presentAnimation.Play(_presentNotAvailableClip.name);
            }
        }
        
        private async void Timer_OnComplete(UITimer sender)
        {
            await NetworkTimeManager.ForceUpdate(Application.exitCancellationToken);
            SetAvailability();
        }
    }
}