using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Core
{
    public class NetworkTimeManager : MonoBehaviour
    {
        private static long _ticksDeltaStamp;
        
        public static bool TimeUpdated { get; private set; }
        public static long NowTicks => DateTime.Now.Ticks + _ticksDeltaStamp;
        
        private CancellationTokenSource _cancellationTokenSource;

        protected void Awake()
        {
            _cancellationTokenSource = new CancellationTokenSource();
        }
        
        protected void OnDestroy()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        private async void Start()
        {
            await UpdateTime(_cancellationTokenSource.Token);
        }

        private async void OnApplicationPause(bool isPaused)
        {
            if (!isPaused)
            {
                await UpdateTime(_cancellationTokenSource.Token);
            }
        }

        private static async Task UpdateTime(CancellationToken cancellationToken)
        {
            var ntpClient = new NtpClient();
            var networkTime = await ntpClient.GetNetworkTimeAsync(cancellationToken);

            TimeUpdated = networkTime.success;
            if (networkTime.success)
            {
                _ticksDeltaStamp = networkTime.time.Ticks - DateTime.Now.Ticks;
            }
        }

        

        public static async Task ForceUpdate(CancellationToken cancellationToken)
        {
            await UpdateTime(cancellationToken);
        }
    }
}