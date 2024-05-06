using System;
using System.Threading;
using System.Threading.Tasks;
using Save;
using UnityEngine;

namespace Core
{
    public class GiftModel : IDisposable
    {
        public event Action<GiftModel, bool> OnCollect;
        
        private readonly string _id;
        private readonly long _collectInterval;
        private readonly int _currencyAmount;
        private readonly SaveProgress _saveProgress;
       
        public string Id => _id;
        public int CurrencyAmount => _currencyAmount;

        public GiftModel(string id, long collectInterval, int currencyAmount, SaveProgress saveProgress)
        {
            _id = id;
            _collectInterval = collectInterval;
            _currencyAmount = currencyAmount;
            
            _saveProgress = saveProgress;
        }
        
        public async Task<(bool success, int amount)> CollectAsync( CancellationToken cancellationToken)
        {
            var ntpClient = new NtpClient();
            var networkTime = await ntpClient.GetNetworkTimeAsync(cancellationToken);

            if (!networkTime.success)
            {
                OnCollect?.Invoke(this, false);
                return (false, 0);
            }

            var canCollect = false;
            
            var giftLastCollectedTimestamp = _saveProgress.GetGiftLastCollectedTimestamp(_id);
            if (giftLastCollectedTimestamp != -1)
            {
                var elapsedTime = networkTime.time.Ticks - giftLastCollectedTimestamp;
                var fromTicks = TimeSpan.FromTicks(elapsedTime);
                var fromTicks_collectInterval = TimeSpan.FromTicks(_collectInterval);

                if (elapsedTime > _collectInterval)
                {
                    canCollect = true;
                }
            }
            else
            {
                canCollect = true;
            }

            if (!canCollect)
            {
                OnCollect?.Invoke(this, false);
                return (false, 0);
            }

            var saveResult = await _saveProgress.SetGiftLastCollectedTimestamp(_id, networkTime.time.Ticks);
            if (!saveResult)
            {
                OnCollect?.Invoke(this, false);
                return (false, 0);
            }

            OnCollect?.Invoke(this, true);
            return (true, _currencyAmount);
        } 
        
        public long GetRestTimeForCollect(long currentTicks)
        {
            var fromDays = TimeSpan.FromDays(1);
            var fromDaysTicks = fromDays.Ticks;
            var giftLastCollectedTimestamp = _saveProgress.GetGiftLastCollectedTimestamp(_id);
            if (giftLastCollectedTimestamp != -1)
            {
                return Math.Max(_collectInterval - (currentTicks - giftLastCollectedTimestamp), 0);
            }
            else
            {
                return 0;
            }
        }
        
        public void Dispose()
        {
            
        }
    }
}