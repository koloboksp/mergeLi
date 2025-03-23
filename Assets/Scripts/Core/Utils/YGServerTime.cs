#if UNITY_WEBGL
using System;
using System.Threading;
using System.Threading.Tasks;
using YG;

namespace Core.Utils
{
    public class YGServerTime : IServerTime
    {
        public async Task<(bool success, DateTime time)> GetTimeAsync(CancellationToken externalCancellationToken)
        {
            while (!YG2.isSDKEnabled)
            {
                await Task.Yield();
            }
            
            var fromMilliseconds = TimeSpan.FromMilliseconds(YG2.ServerTime());
            var current = DateTime.UnixEpoch + fromMilliseconds;
            return (true, current);
        }
    }
}
#endif