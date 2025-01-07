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
            return (true, new DateTime(YG2.ServerTime()));
        }
    }
}