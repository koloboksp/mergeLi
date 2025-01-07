using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Utils
{
    public interface IServerTime
    { 
        Task<(bool success, DateTime time)> GetTimeAsync(CancellationToken externalCancellationToken);
    }
}