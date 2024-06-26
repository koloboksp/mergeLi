using System.Threading;
using System.Threading.Tasks;

namespace Core
{
    public interface IAdsController
    {
        Task InitializeAsync();
        Task<bool> Show(AdvertisingType adType, CancellationToken cancellationToken);
    }
    
    public enum AdvertisingType
    {
        Rewarded,
        Interstitial,
    }
}