using System.Threading.Tasks;

namespace Core
{
    public interface IAdsController
    {
        Task InitializeAsync();
        Task<bool> Show(AdvertisingType adType);
    }
    
    public enum AdvertisingType
    {
        Rewarded,
        Interstitial,
    }
}