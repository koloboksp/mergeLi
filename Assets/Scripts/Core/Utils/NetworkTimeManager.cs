using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Utils
{
    public class NetworkTimeManager : MonoBehaviour
    {
        private static long _ticksDeltaStamp;
        
        public static bool TimeUpdated { get; private set; }
        public static long NowTicks => DateTime.Now.Ticks + _ticksDeltaStamp;

        private NetworkReachability _networkReachability = NetworkReachability.NotReachable;
        
        private async void Start()
        {
            await UpdateTime(Application.exitCancellationToken);
        }

        private async void OnApplicationPause(bool isPaused)
        {
            if (!isPaused)
            {
                await UpdateTime(Application.exitCancellationToken);
            }
        }

        private async void Update()
        {
            if (_networkReachability != Application.internetReachability)
            {
                _networkReachability = Application.internetReachability;
                await UpdateTime(Application.exitCancellationToken);
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